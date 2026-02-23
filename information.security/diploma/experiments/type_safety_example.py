class ClassA:

    def do(self, data):
        print(f"in ClassA.do: {data}")

class ClassB:

    def do(self, data):
        print(f"in ClassB.do: {data}")

def make_unsafe(objA):
    print("In make_unsafe")
    objA.do("IDDQD")

def make_safe(objA):
    print("In make_safe")
    if not isinstance(objA, ClassA):
        raise ValueError("obj is not ClassA instance")
    objA.do("IDKFA")

if __name__ == "__main__":
    objA = ClassA()
    objB = ClassB()
    make_unsafe(objA)
    make_unsafe(objB)
    make_safe(objA)
    make_safe(objB)