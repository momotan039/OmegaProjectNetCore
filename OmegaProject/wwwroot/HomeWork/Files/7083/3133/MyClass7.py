class  MyColor:

    def  __init__(self,color):
        self.__color=color

    def getColor():
        return self.__color

    def setColor(self,color):
        self.__color=color

c=MyColor("red")
print(c.getColor())

c.setColor("green")
print(c.getColor())
