namespace ExperimentExample1
{
    public abstract class Step
    {
        protected abstract Boolean Check();

        public abstract String GetRepresentation();
    }

    public class Move : Step
    {
        public Move(Int32 xPosValue, Int32 yPosValue)
        {
            xPos = xPosValue;
            yPos = yPosValue;
        }

        protected override Boolean Check()
        {
            return (xPos >= 0) && (yPos >= 0);
        }

        public override String GetRepresentation()
        {
            if (!Check())
                throw new InvalidOperationException();
            return $"move({xPos},{yPos})";
        }

        private readonly Int32 xPos;
        private readonly Int32 yPos;
    }

    public class Color : Step
    {
        public Color(String colorValue)
        {
            color = colorValue;
        }

        protected override Boolean Check()
        {
            return (color != null) && (color.Length > 0);
        }

        public override String GetRepresentation()
        {
            if (!Check())
                throw new InvalidOperationException();
            return $"color({color})";
        }

        private readonly String color;
    }

    public class Point : Step
    {
        public Point(Int32 xPosValue, Int32 yPosValue)
        {
            xPos = xPosValue;
            yPos = yPosValue;
        }

        protected override Boolean Check()
        {
            return (xPos >= 0) && (yPos >= 0);
        }

        public override String GetRepresentation()
        {
            if (!Check())
                throw new InvalidOperationException();
            return $"point({xPos},{yPos})";
        }

        private readonly Int32 xPos;
        private readonly Int32 yPos;
    }

    public class Rectangle : Step
    {
        public Rectangle(Int32 xLeftValue, Int32 yTopValue, Int32 xRightValue,Int32 yBottomValue)
        {
            xLeft = xLeftValue;
            yTop = yTopValue;
            xRight = xRightValue;
            yBottom = yBottomValue;
        }

        public Rectangle(Int32 xLeftValue, Int32 yTopValue, Int32 size)
        {
            xLeft = xLeftValue;
            yTop = yTopValue;
            xRight = xLeftValue + size;
            yBottom = yTopValue + size;
        }

        protected override Boolean Check()
        {
            return (xLeft >= 0) && (yTop >= 0) && (xRight >= 0) && (yBottom >= 0) && (xRight > xLeft) && (yBottom > yTop);
        }

        public override String GetRepresentation()
        {
            if (!Check())
                throw new InvalidOperationException();
            return $"rectangle({xLeft},{yTop},{xRight},{yBottom})";
        }

        private readonly Int32 xLeft;
        private readonly Int32 yTop;
        private readonly Int32 xRight;
        private readonly Int32 yBottom;
    }

    public class Circle : Step
    {
        public Circle(Int32 xCenterValue, Int32 yCenterValue, Int32 radiusValue)
        {
            xCenter = xCenterValue;
            yCenter = yCenterValue;
            radius = radiusValue;
        }

        protected override Boolean Check()
        {
            return (xCenter >= 0) && (yCenter >= 0) && (radius > 0);
        }

        public override String GetRepresentation()
        {
            if (!Check())
                throw new InvalidOperationException();
            return $"circle({xCenter},{yCenter},{radius})";
        }

        private readonly Int32 xCenter;
        private readonly Int32 yCenter;
        private readonly Int32 radius;
    }

    public class StepStorage
    {
        public StepStorage()
        {
            steps = new List<Step>();
        }

        public IList<Step> GetSteps()
        {
            return steps;
        }

        public void AddMove(Move move)
        {
            steps.Add(move);
        }

        public void AddMove(Int32 x, Int32 y)
        {
            steps.Add(new Move(x, y));
        }

        public void AddColor(Color color)
        {
            steps.Add(color);
        }

        public void AddColor(String color)
        {
            steps.Add(new Color(color));
        }

        public void AddPoint(Point point)
        {
            steps.Add(point);
        }

        public void AddPoint(Int32 x, Int32 y)
        {
            steps.Add(new Point(x, y));
        }

        public void AddRectangle(Rectangle rectangle)
        {
            steps.Add(rectangle);
        }

        public void AddRectangle(Int32 xLeft, Int32 yTop, Int32 xRight, Int32 yBottom)
        {
            steps.Add(new Rectangle(xLeft, yTop, xRight, yBottom));
        }

        public void AddRectangle(Int32 xLeft, Int32 yTop, Int32 size)
        {
            steps.Add(new Rectangle(xLeft, yTop, size));
        }

        public void AddCircle(Circle circle)
        {
            steps.Add(circle);
        }

        public void AddCircle(Int32 xCenter, Int32 yCenter, Int32 radius)
        {
            steps.Add(new Circle(xCenter, yCenter, radius));
        }

        private IList<Step> steps;
    }

    public interface IPainter
    {
        void Paint(IList<Step> steps);
    }

    public class SimplePainter : IPainter
    {
        public void Paint(IList<Step> steps)
        {
            foreach (Step step in steps)
            {
                Console.WriteLine(step.GetRepresentation());
            }
        }
    }

    internal class Program
    {
        static void Main(String[] args)
        {
            StepStorage storage = new StepStorage();
            storage.AddMove(3, 5);
            storage.AddColor("red");
            storage.AddPoint(10, 10);
            storage.AddMove(100,100);
            storage.AddRectangle(12,14,55);
            storage.AddCircle(6,6,77);
            IPainter simplePainter = new SimplePainter();
            simplePainter.Paint(storage.GetSteps());
        }
    }
}
