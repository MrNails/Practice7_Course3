using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Practice_7_MyWorld
{
    enum EatState : byte
    {
        Normal = 1,
        Eated
    }

    enum AnimalState : byte
    {
        Alive = 1,
        Dead
    }

    interface IWorldObject
    {
        int X { get; set; }
        int Y { get; set; }
        Rectangle Model { get; }

        bool CheckIntersection(IWorldObject outerObject);
    }

    interface IEat
    {
        EatState EatState { get; set; }
    }

    abstract class Animal : IWorldObject
    {
        protected int x;
        protected int y;
        protected int lifeTime;
        protected int maxLifeTime;
        protected int lifeTimeWithoutEat;
        protected int maxLifeTimeWithoutEat;
        protected int timeToHunger;
        protected int maxTimeToHunger;
        protected int reproductionCoolDown;
        protected Rectangle model;
        protected Canvas field;
        protected AnimalState animalState;
        protected Random random;

        public Animal() : this(null, 50, 30, 10)
        { }
        public Animal(Canvas field, int lifeTime, int lifeTimeWithoutEat, int timeToHunger)
        {
            if (field == null)
            {
                this.field = new Canvas();
            }
            else
            {
                this.field = field;
            }

            LifeTime = lifeTime;
            LifeTimeWithoutEat = lifeTimeWithoutEat;
            TimeToHunger = timeToHunger;

            maxLifeTime = LifeTime;
            maxTimeToHunger = TimeToHunger;
            maxLifeTimeWithoutEat = LifeTimeWithoutEat;

            ReproductionCoolDown = 0;

            AnimalState = AnimalState.Alive;

            random = new Random(DateTime.Now.Millisecond);
        }

        public Rectangle Model { get { return model; } }
        public Canvas Field { get { return field; } }

        public int LifeTime
        {
            get { return lifeTime; }
            protected set
            {
                if (AnimalState == AnimalState.Dead)
                {
                    lifeTime = 0;
                    return;
                }

                if (value < 0)
                {
                    lifeTime = 0;
                    AnimalState = AnimalState.Dead;
                }
                else
                {
                    lifeTime = value;
                }
            }
        }
        public int LifeTimeWithoutEat
        {
            get { return lifeTimeWithoutEat; }
            protected set
            {
                if (AnimalState == AnimalState.Dead)
                {
                    lifeTimeWithoutEat = 0;
                    return;
                }

                if (value < 0)
                {
                    lifeTimeWithoutEat = 0;
                    AnimalState = AnimalState.Dead;
                }
                else
                {
                    lifeTimeWithoutEat = value;
                }
            }
        }
        public int TimeToHunger
        {
            get { return timeToHunger; }
            protected set
            {
                if (AnimalState == AnimalState.Dead)
                {
                    timeToHunger = 0;
                    return;
                }

                if (value < 0)
                {
                    timeToHunger = 0;
                    IsFeedUp = false;
                }
                else
                {
                    timeToHunger = value;
                }
            }
        }
        public int ReproductionCoolDown
        {
            get { return reproductionCoolDown; }
            set
            {
                if (value < 0)
                {
                    reproductionCoolDown = 0;
                }
                else
                {
                    reproductionCoolDown = value;
                }
            }
        }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public virtual AnimalState AnimalState
        {
            get { return animalState; }
            set
            {
                if (animalState == AnimalState.Dead)
                {
                    return;
                }

                animalState = value;
            }
        }

        public bool IsFeedUp { get; set; }

        public bool CheckIntersection(IWorldObject outerObject)
        {
            if (outerObject == null)
            {
                return false;
            }

            if (X < outerObject.X && X + Model.Width > outerObject.X &&
                Y < outerObject.Y && Y + Model.Height > outerObject.Y)
            {
                return true;
            }

            if (X < outerObject.X + outerObject.Model.Width && X + Model.Width > outerObject.X + outerObject.Model.Width &&
                Y < outerObject.Y + outerObject.Model.Height && Y + Model.Height > outerObject.Y + outerObject.Model.Height)
            {
                return true;
            }

            if (X < outerObject.X && X + Model.Width > outerObject.X &&
                Y < outerObject.Y + outerObject.Model.Height && Y + Model.Height > outerObject.Y + outerObject.Model.Height)
            {
                return true;
            }

            if (X < outerObject.X + outerObject.Model.Width && X + Model.Width > outerObject.X + outerObject.Model.Width &&
                Y < outerObject.Y && Y + Model.Height > outerObject.Y)
            {
                return true;
            }


            return false;
        }

        public virtual void Move()
        {
            if (AnimalState == AnimalState.Dead)
            {
                return;
            }

            Random stepLength = new Random((int)DateTime.Now.Ticks);
            int deltaX = random.Next(-(int)(Model.Width * stepLength.NextDouble()), (int)(Model.Width * stepLength.NextDouble()));
            int deltaY = random.Next(-(int)(Model.Height * stepLength.NextDouble()), (int)(Model.Height * stepLength.NextDouble()));

            x += deltaX;
            y += deltaY;

            if (x < 0)
            {
                x = 0;
            }
            else if (x > Field.ActualWidth)
            {
                x = (int)(Field.ActualWidth - Model.Width);
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y > Field.ActualHeight)
            {
                y = (int)(Field.ActualHeight - Model.Height);
            }

            LifeTime -= 1;
            LifeTimeWithoutEat -= 1;
            TimeToHunger -= 1;

            ReproductionCoolDown -= 1;

            Canvas.SetLeft(Model, x);
            Canvas.SetTop(Model, y);
        }
        public virtual void ToEat(IEat eat)
        {
            if (AnimalState == AnimalState.Dead || IsFeedUp)
            {
                return;
            }

            eat.EatState = EatState.Eated;
            IsFeedUp = true;

            LifeTimeWithoutEat = maxLifeTimeWithoutEat;
            TimeToHunger = maxTimeToHunger;
        }

        public abstract Animal Reproduction(Animal animal);

    }

    public class World
    {
        class Wolf : Animal
        {
            public Wolf() : this(null, 50, 30, 10)
            { }
            public Wolf(Canvas field, int lifeTime, int lifeTimeWithoutEat, int timeToHunger) : base(field, lifeTime, lifeTimeWithoutEat, timeToHunger)
            {
                model = new Rectangle();
                model.Fill = new SolidColorBrush(Colors.Gray);
                model.Width = 50;
                model.Height = 50;

                field.Children.Add(model);
            }

            public override AnimalState AnimalState
            {
                get => base.AnimalState;
                set
                {
                    if (value == AnimalState.Dead)
                    {
                        model.Fill = new SolidColorBrush(Colors.DarkGray);
                    }

                    base.AnimalState = value;
                }
            }

            public override Animal Reproduction(Animal animal)
            {
                if (animalState == AnimalState.Dead || animal.AnimalState == AnimalState.Dead || !(animal is Sheep) || 
                    ReproductionCoolDown != 0 || animal.ReproductionCoolDown != 0)
                {
                    return null;
                }

                if (IsFeedUp && animal.IsFeedUp)
                {
                    Wolf newWolf = new Wolf(Field, maxLifeTime, maxLifeTimeWithoutEat, maxTimeToHunger);

                    IsFeedUp = false;
                    animal.IsFeedUp = false;

                    newWolf.X = (X + animal.X) / 2;
                    newWolf.Y = (Y + animal.Y) / 2;

                    ReproductionCoolDown = (int)(LifeTime * 0.1);
                    animal.ReproductionCoolDown = (int)(animal.LifeTime * 0.1);

                    return newWolf;
                }

                return null;
            }
            public override void ToEat(IEat eat)
            {
                if (eat is Sheep)
                {
                    base.ToEat(eat);
                }
            }
        }

        class Sheep : Animal, IEat
        {
            private EatState eatState;

            public Sheep() : this(null, 50, 30, 10)
            { }
            public Sheep(Canvas field, int lifeTime, int lifeTimeWithoutEat, int timeToHunger) : base(field, lifeTime, lifeTimeWithoutEat, timeToHunger)
            {
                model = new Rectangle();
                model.Stroke = new SolidColorBrush(Colors.Black);
                model.Fill = new SolidColorBrush(Colors.White);
                model.Width = 50;
                model.Height = 50;

                field.Children.Add(model);
            }

            public EatState EatState
            {
                get
                {
                    return eatState;
                }
                set
                {
                    if (value == EatState.Eated)
                    {
                        AnimalState = AnimalState.Dead;
                    }

                    eatState = value;
                }
            }

            public override AnimalState AnimalState
            {
                get => base.AnimalState;
                set
                {
                    if (value == AnimalState.Dead)
                    {
                        model.Fill = new SolidColorBrush(Colors.DarkGray);
                    }

                    base.AnimalState = value;
                }
            }

            public override Animal Reproduction(Animal animal)
            {
                if (animalState == AnimalState.Dead || animal.AnimalState == AnimalState.Dead || !(animal is Sheep) ||
                    ReproductionCoolDown != 0 || animal.ReproductionCoolDown != 0)
                {
                    return null;
                }

                if (IsFeedUp && animal.IsFeedUp)
                {
                    Sheep newSheep = new Sheep(Field, maxLifeTime, maxLifeTimeWithoutEat, maxTimeToHunger);

                    IsFeedUp = false;
                    animal.IsFeedUp = false;

                    newSheep.X = (X + animal.X) / 2;
                    newSheep.Y = (Y + animal.Y) / 2;

                    ReproductionCoolDown = (int)(LifeTime * 0.05);
                    animal.ReproductionCoolDown = (int)(animal.LifeTime * 0.05);

                    return newSheep;
                }

                return null;
            }
            public override void ToEat(IEat eat)
            {
                if (eat is Grass)
                {
                    base.ToEat(eat);
                }
            }
        }

        class Grass : IEat, IWorldObject
        {
            private int x;
            private int y;
            private Rectangle model;
            private EatState eatState;
            private Canvas field;

            public Grass() : this(new Canvas(), 0, 0)
            { }
            public Grass(Canvas field, int x, int y)
            {
                model = new Rectangle();
                model.Fill = new SolidColorBrush(Colors.Green);
                model.Width = 50;
                model.Height = 50;

                this.field = field;

                field.Children.Insert(0, model);

                X = x;
                Y = y;
            }

            public Rectangle Model { get { return model; } }

            public int X
            {
                get
                {
                    return x;
                }
                set
                {
                    //По идее, должно работать как выстраивание под сетку
                    if (value % (int)Model.Width < (int)Model.Width / 2)
                    {
                        x = value - (value % (int)Model.Width);
                    }
                    else
                    {
                        if (value - (value % (int)Model.Width) + (int)Model.Width < field.ActualWidth)
                        {
                            x = value - (value % (int)Model.Width) + (int)Model.Width;
                        }
                        else
                        {
                            x = value - (value % (int)Model.Width);
                        }
                    }

                    Canvas.SetLeft(Model, x);
                }
            }
            public int Y
            {
                get
                {
                    return y;
                }
                set
                {
                    //По идее, должно работать как выстраивание под сетку
                    if (value % (int)Model.Height < (int)Model.Height / 2)
                    {
                        y = value - (value % (int)Model.Height);
                    }
                    else
                    {
                        if (value - (value % (int)Model.Height) + (int)Model.Height < field.ActualHeight)
                        {
                            y = value - (value % (int)Model.Height) + (int)Model.Height;
                        }
                        else
                        {
                            y = value - (value % (int)Model.Height);
                        }
                    }
                    Canvas.SetTop(Model, y);
                }
            }

            public EatState EatState
            {
                get
                {
                    return eatState;
                }
                set
                {
                    eatState = value;
                }
            }

            public bool CheckIntersection(IWorldObject outerObject)
            {
                if (X <= outerObject.X && X + Model.Width > outerObject.X &&
                    Y <= outerObject.Y && Y + Model.Height > outerObject.Y)
                {
                    return true;
                }

                if (X <= outerObject.X + outerObject.Model.Width && X + Model.Width > outerObject.X + outerObject.Model.Width &&
                    Y <= outerObject.Y + outerObject.Model.Height && Y + Model.Height > outerObject.Y + outerObject.Model.Height)
                {
                    return true;
                }

                return false;
            }
        }

        private bool fieldIsFill;
        private double worldSpeed;
        private int speedMultiplier;
        private int grassSpawnRepeatCount;
        private List<IWorldObject> worldObjects;
        private List<IWorldObject> eatedOrDeadObjects;
        private bool[,] grassField;
        private Canvas field;
        private DispatcherTimer dispatcherTimer;
        private Random random;

        public World() : this(new Canvas())
        {
        }

        public World(Canvas field)
        {
            this.field = field;

            random = new Random(DateTime.Now.Millisecond);

            worldObjects = new List<IWorldObject>();
            eatedOrDeadObjects = new List<IWorldObject>();

            Wolf wolf1 = new Wolf(field, 1000, 600, 300);
            Wolf wolf2 = new Wolf(field, 1000, 600, 300);
            Sheep sheep1 = new Sheep(field, 1000, 600, 300);
            Sheep sheep2 = new Sheep(field, 1000, 600, 300);
            Grass grass = new Grass();

            wolf1.X = 100;
            wolf1.Y = 100;
            wolf2.X = 150;
            wolf2.Y = 120;
            sheep1.X = 300;
            sheep1.Y = 300;
            sheep2.X = 360;
            sheep2.Y = 300;

            worldObjects.AddRange(new List<IWorldObject> { wolf1, wolf2, sheep1, sheep2, grass });

            ////Определяет количество элементов в зависимости от размеров формы
            grassField = new bool[(int)(field.ActualWidth / grass.Model.Width) + 1, (int)(field.ActualHeight / grass.Model.Height) + 1];

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Action;

            worldSpeed = 1;
            SpeedMultiplier = 1;
        }

        public int SpeedMultiplier
        {
            get { return speedMultiplier; }
            set 
            {
                if (value > 0)
                {
                    speedMultiplier = value * 10;

                    dispatcherTimer.Interval = TimeSpan.FromSeconds(worldSpeed / speedMultiplier);
                }
            }
        }


        public void StartSimulation()
        {
            dispatcherTimer.Start();
        }
        public void StopSimulation()
        {
            dispatcherTimer.Stop();
        }

        private void Action(object obj, EventArgs arg)
        {
            if (!fieldIsFill)
            {
                int grassCount = random.Next(1, 4);
                Grass _grass = null;

                //Создание травы
                do
                {
                    _grass = new Grass(field, random.Next(0, (int)field.ActualWidth), random.Next(0, (int)field.ActualHeight));

                    //Сначала идёт заполнение с помощью рандома, а потом уже через последовательное заполнение
                    for (int i = 0; i < worldObjects.Count; i++)
                    {
                        if (grassSpawnRepeatCount > 50)
                        {
                            fieldIsFill = !FillLogicField(grassField, _grass, true);

                            if (fieldIsFill)
                            {
                                grassCount = 0;
                                _grass = null;
                            }

                            break;
                        }
                        else
                        {
                            if (worldObjects[i] is Grass && worldObjects[i].CheckIntersection(_grass))
                            {
                                i = -1;
                                _grass.X = random.Next(0, (int)field.ActualWidth);
                                _grass.Y = random.Next(0, (int)field.ActualHeight);
                                grassSpawnRepeatCount++;
                            }
                        }
                    }

                    if (_grass != null)
                    {
                        FillLogicField(grassField, _grass, false);
                        worldObjects.Add(_grass);
                        grassSpawnRepeatCount = 0;
                        grassCount--;
                    }

                } while (grassCount > 0);
            }


            //Проход по всей коллекции с дальнейшей проверкой на пересечение и вызовом методов ToEat(), Reproduction()
            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (worldObjects[i] is IEat && ((IEat)worldObjects[i]).EatState == EatState.Eated)
                {
                    eatedOrDeadObjects.Add(worldObjects[i]);
                    continue;
                }

                if (worldObjects[i] is Animal)
                {
                    if (((Animal)worldObjects[i]).AnimalState == AnimalState.Dead)
                    {
                        eatedOrDeadObjects.Add(worldObjects[i]);
                        continue;
                    }

                    ((Animal)worldObjects[i]).Move();

                    //Проверка пересечения с остальными объектами поля
                    for (int j = 0; j < worldObjects.Count; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        if (worldObjects[i].CheckIntersection(worldObjects[j]))
                        {
                            //Сначала проверяется, может ли объект, с которым проверяют учавствовать в репродукции
                            if (worldObjects[j] is Animal && ((Animal)worldObjects[j]).AnimalState != AnimalState.Dead)
                            {
                                Animal animal = ((Animal)worldObjects[i]).Reproduction((Animal)worldObjects[j]);
                                if (animal != null)
                                {
                                    worldObjects.Add(animal);
                                }
                            }

                            if (worldObjects[j] is IEat && ((IEat)worldObjects[j]).EatState != EatState.Eated)
                            {
                                ((Animal)worldObjects[i]).ToEat((IEat)worldObjects[j]);
                            }
                        }
                    }
                }
            }

            //Убирает объекты с состоянием Eated
            foreach (var _obj in eatedOrDeadObjects)
            {
                grassSpawnRepeatCount = 0;

                if (_obj is Grass)
                {
                    if (RemoveFromLogicField(grassField, _obj))
                    {
                        fieldIsFill = false;
                    }
                }

                field.Children.Remove(_obj.Model);
                worldObjects.Remove(_obj);
            }

            eatedOrDeadObjects.Clear();
        }
        
        //Аргумент allField означает поиск пустых ячеек в логическом поле и устанавливает координаты объекту
        private bool FillLogicField(bool[,] field, IWorldObject obj, bool allField)
        {
            int row = obj.X / (int)obj.Model.Width;
            int column = obj.Y / (int)obj.Model.Height;

            if (row > obj.Model.Width || column > obj.Model.Height)
            {
                return false;
            }

            //Заполняет логическую ячейку по координатам
            if (!field[row, column])
            {
                field[row, column] = true;
                return true;
            }

            if (allField)
            {
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (!field[i, j])
                        {
                            field[i, j] = true;
                            obj.X = (int)obj.Model.Width * i;
                            obj.Y = (int)obj.Model.Height * j;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private bool RemoveFromLogicField(bool[,] field, IWorldObject obj)
        {
            if (field[obj.X / (int)obj.Model.Width, obj.Y / (int)obj.Model.Height])
            {
                field[obj.X / (int)obj.Model.Width, obj.Y / (int)obj.Model.Height] = false;
                return true;
            }

            return false;
        }
    }
}
