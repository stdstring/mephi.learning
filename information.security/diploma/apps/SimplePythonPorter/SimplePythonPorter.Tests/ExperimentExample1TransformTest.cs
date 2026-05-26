using NUnit.Framework;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.Processor;

namespace SimplePythonPorter.Tests
{
    [TestFixture]
    public class ExperimentExample1TransformTest
    {
        [Test]
        public void Check()
        {
            const String experimentExample1Project = "..\\..\\..\\..\\ExperimentExample1\\ExperimentExample1.csproj";
            AppData appData = new AppData(NameTransformer:new NameTransformer(),
                                          Results: new List<TransformResult>());
            ProjectProcessor processor = new ProjectProcessor(appData);
            processor.Process(experimentExample1Project);
            Assert.That(appData.Results.Count, Is.EqualTo(1));
            Assert.That(appData.Results[0].Content, Is.EqualTo(ExperimentExample1Result));
        }

        private const String ExperimentExample1Result =
            "# -*- coding: utf-8 -*-\r\n" +
            "\r\n" +
            "import system\r\n" +
            "import system.collections.generic\r\n" +
            "from abc import ABC\r\n" +
            "\r\n" +
            "\r\n" +
            "class Step(ABC):\r\n" +
            "    def __init__(self):\r\n" +
            "        pass\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        raise system.InvalidOperationException(\"Abstract method call\")\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        raise system.InvalidOperationException(\"Abstract method call\")\r\n" +
            "\r\n" +
            "class Move(Step):\r\n" +
            "    def __init__(self, xPosValue, yPosValue):\r\n" +
            "        self.__xPos = 0\r\n" +
            "        self.__yPos = 0\r\n" +
            "        if isinstance(xPosValue, int) and isinstance(yPosValue, int):\r\n" +
            "            self.__xPos = xPosValue\r\n" +
            "            self.__yPos = yPosValue\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        return (self.__xPos >= 0) and (self.__yPos >= 0)\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        if not self._Check():\r\n" +
            "            raise system.InvalidOperationException(\"Bad data\")\r\n" +
            "        return f\"move({self.__xPos},{self.__yPos})\"\r\n" +
            "\r\n" +
            "class Color(Step):\r\n" +
            "    def __init__(self, colorValue):\r\n" +
            "        self.__color = \"\"\r\n" +
            "        if (isinstance(colorValue, str) or colorValue is None):\r\n" +
            "            self.__color = colorValue\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        return (self.__color != None) and (len(self.__color) > 0)\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        if not self._Check():\r\n" +
            "            raise system.InvalidOperationException(\"Bad data\")\r\n" +
            "        return f\"color({self.__color})\"\r\n" +
            "\r\n" +
            "class Point(Step):\r\n" +
            "    def __init__(self, xPosValue, yPosValue):\r\n" +
            "        self.__xPos = 0\r\n" +
            "        self.__yPos = 0\r\n" +
            "        if isinstance(xPosValue, int) and isinstance(yPosValue, int):\r\n" +
            "            self.__xPos = xPosValue\r\n" +
            "            self.__yPos = yPosValue\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        return (self.__xPos >= 0) and (self.__yPos >= 0)\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        if not self._Check():\r\n" +
            "            raise system.InvalidOperationException(\"Bad data\")\r\n" +
            "        return f\"point({self.__xPos},{self.__yPos})\"\r\n" +
            "\r\n" +
            "class Rectangle(Step):\r\n" +
            "    def __init__(self, *args):\r\n" +
            "        self.__xLeft = 0\r\n" +
            "        self.__yTop = 0\r\n" +
            "        self.__xRight = 0\r\n" +
            "        self.__yBottom = 0\r\n" +
            "        if (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):\r\n" +
            "            xLeftValue = args[0]\r\n" +
            "            yTopValue = args[1]\r\n" +
            "            size = args[2]\r\n" +
            "            self.__xLeft = xLeftValue\r\n" +
            "            self.__yTop = yTopValue\r\n" +
            "            self.__xRight = xLeftValue + size\r\n" +
            "            self.__yBottom = yTopValue + size\r\n" +
            "        elif (len(args) == 4) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int) and isinstance(args[3], int):\r\n" +
            "            xLeftValue = args[0]\r\n" +
            "            yTopValue = args[1]\r\n" +
            "            xRightValue = args[2]\r\n" +
            "            yBottomValue = args[3]\r\n" +
            "            self.__xLeft = xLeftValue\r\n" +
            "            self.__yTop = yTopValue\r\n" +
            "            self.__xRight = xRightValue\r\n" +
            "            self.__yBottom = yBottomValue\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        return (self.__xLeft >= 0) and (self.__yTop >= 0) and (self.__xRight >= 0) and (self.__yBottom >= 0) and (self.__xRight > self.__xLeft) and (self.__yBottom > self.__yTop)\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        if not self._Check():\r\n" +
            "            raise system.InvalidOperationException(\"Bad data\")\r\n" +
            "        return f\"rectangle({self.__xLeft},{self.__yTop},{self.__xRight},{self.__yBottom})\"\r\n" +
            "\r\n" +
            "class Circle(Step):\r\n" +
            "    def __init__(self, xCenterValue, yCenterValue, radiusValue):\r\n" +
            "        self.__xCenter = 0\r\n" +
            "        self.__yCenter = 0\r\n" +
            "        self.__radius = 0\r\n" +
            "        if isinstance(xCenterValue, int) and isinstance(yCenterValue, int) and isinstance(radiusValue, int):\r\n" +
            "            self.__xCenter = xCenterValue\r\n" +
            "            self.__yCenter = yCenterValue\r\n" +
            "            self.__radius = radiusValue\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def _Check(self):\r\n" +
            "        minX = self.__xCenter - self.__radius\r\n" +
            "        minY = self.__yCenter - self.__radius\r\n" +
            "        return (self.__xCenter >= 0) and (self.__yCenter >= 0) and (self.__radius > 0) and (minX >= 0) and (minY >= 0)\r\n" +
            "\r\n" +
            "    def GetRepresentation(self):\r\n" +
            "        if not self._Check():\r\n" +
            "            raise system.InvalidOperationException(\"Bad data\")\r\n" +
            "        return f\"circle({self.__xCenter},{self.__yCenter},{self.__radius})\"\r\n" +
            "\r\n" +
            "class StepStorage:\r\n" +
            "    def __init__(self):\r\n" +
            "        self.__steps = None\r\n" +
            "        self.__steps = system.collections.generic.List()\r\n" +
            "\r\n" +
            "    def AddCircle(self, *args):\r\n" +
            "        if (len(args) == 1) and (isinstance(args[0], Circle) or args[0] is None):\r\n" +
            "            circle = args[0]\r\n" +
            "            if circle == None:\r\n" +
            "                raise system.ArgumentNullException(\"circle\")\r\n" +
            "            self.__steps.Add(circle)\r\n" +
            "        elif (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):\r\n" +
            "            xCenter = args[0]\r\n" +
            "            yCenter = args[1]\r\n" +
            "            radius = args[2]\r\n" +
            "            self.__steps.Add(Circle(xCenter, yCenter, radius))\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def AddColor(self, *args):\r\n" +
            "        if (len(args) == 1) and (isinstance(args[0], Color) or args[0] is None):\r\n" +
            "            color = args[0]\r\n" +
            "            if color == None:\r\n" +
            "                raise system.ArgumentNullException(\"color\")\r\n" +
            "            self.__steps.Add(color)\r\n" +
            "        elif (len(args) == 1) and (isinstance(args[0], str) or args[0] is None):\r\n" +
            "            color = args[0]\r\n" +
            "            if color == None:\r\n" +
            "                raise system.ArgumentNullException(\"color\")\r\n" +
            "            self.__steps.Add(Color(color))\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def AddMove(self, *args):\r\n" +
            "        if (len(args) == 1) and (isinstance(args[0], Move) or args[0] is None):\r\n" +
            "            move = args[0]\r\n" +
            "            if move == None:\r\n" +
            "                raise system.ArgumentNullException(\"move\")\r\n" +
            "            self.__steps.Add(move)\r\n" +
            "        elif (len(args) == 2) and isinstance(args[0], int) and isinstance(args[1], int):\r\n" +
            "            x = args[0]\r\n" +
            "            y = args[1]\r\n" +
            "            self.__steps.Add(Move(x, y))\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def AddPoint(self, *args):\r\n" +
            "        if (len(args) == 1) and (isinstance(args[0], Point) or args[0] is None):\r\n" +
            "            point = args[0]\r\n" +
            "            if point == None:\r\n" +
            "                raise system.ArgumentNullException(\"point\")\r\n" +
            "            self.__steps.Add(point)\r\n" +
            "        elif (len(args) == 2) and isinstance(args[0], int) and isinstance(args[1], int):\r\n" +
            "            x = args[0]\r\n" +
            "            y = args[1]\r\n" +
            "            self.__steps.Add(Point(x, y))\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def AddRectangle(self, *args):\r\n" +
            "        if (len(args) == 1) and (isinstance(args[0], Rectangle) or args[0] is None):\r\n" +
            "            rectangle = args[0]\r\n" +
            "            if rectangle == None:\r\n" +
            "                raise system.ArgumentNullException(\"rectangle\")\r\n" +
            "            self.__steps.Add(rectangle)\r\n" +
            "        elif (len(args) == 3) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int):\r\n" +
            "            xLeft = args[0]\r\n" +
            "            yTop = args[1]\r\n" +
            "            size = args[2]\r\n" +
            "            self.__steps.Add(Rectangle(xLeft, yTop, size))\r\n" +
            "        elif (len(args) == 4) and isinstance(args[0], int) and isinstance(args[1], int) and isinstance(args[2], int) and isinstance(args[3], int):\r\n" +
            "            xLeft = args[0]\r\n" +
            "            yTop = args[1]\r\n" +
            "            xRight = args[2]\r\n" +
            "            yBottom = args[3]\r\n" +
            "            self.__steps.Add(Rectangle(xLeft, yTop, xRight, yBottom))\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "    def GetSteps(self):\r\n" +
            "        return self.__steps\r\n" +
            "\r\n" +
            "class IPainter(ABC):\r\n" +
            "    def Paint(self, steps):\r\n" +
            "        if (isinstance(steps, system.collections.generic.IList) or steps is None):\r\n" +
            "            raise system.InvalidOperationException(\"Abstract method call\")\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "class SimplePainter(IPainter):\r\n" +
            "    def __init__(self):\r\n" +
            "        pass\r\n" +
            "\r\n" +
            "    def Paint(self, steps):\r\n" +
            "        if (isinstance(steps, system.collections.generic.IList) or steps is None):\r\n" +
            "            if steps == None:\r\n" +
            "                raise system.ArgumentNullException(\"steps\")\r\n" +
            "            for step in steps:\r\n" +
            "                system.Console.WriteLine(step.GetRepresentation())\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n" +
            "\r\n" +
            "class Program:\r\n" +
            "    def __init__(self):\r\n" +
            "        pass\r\n" +
            "\r\n" +
            "    @staticmethod\r\n" +
            "    def Main(args):\r\n" +
            "        if (isinstance(args, list) or args is None):\r\n" +
            "            storage = StepStorage()\r\n" +
            "            storage.AddMove(3, 5)\r\n" +
            "            storage.AddColor(\"red\")\r\n" +
            "            storage.AddPoint(10, 10)\r\n" +
            "            storage.AddMove(100, 100)\r\n" +
            "            storage.AddRectangle(12, 14, 55)\r\n" +
            "            storage.AddCircle(16, 16, 13)\r\n" +
            "            simplePainter = SimplePainter()\r\n" +
            "            simplePainter.Paint(storage.GetSteps())\r\n" +
            "        else:\r\n" +
            "            raise system.InvalidOperationException(\"Unrecognized combination of arguments\")\r\n";
    }
}
