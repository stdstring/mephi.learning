class Console:
    @staticmethod
    def Write(value: object = "") -> None:
        print(value, end="")
    
    @staticmethod
    def WriteLine(value: object = "") -> None:
        print(value)