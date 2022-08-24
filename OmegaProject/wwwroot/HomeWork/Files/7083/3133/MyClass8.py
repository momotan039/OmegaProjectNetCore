class Circle:


    
    def __init__(self,num,color):
        self.__radius=num
        self.color=color

    def getRadius(self):
        return self.__radius


    def setRadius(self,num):
        self.__radius=num

    def Hikaf(self):
        return 3.14*self.__radius

A=Circle(5,"red")
print(A.getRadius(),A.color)

A.setRadius(8)
A.color="yellow"
print(A.getRadius(),A.color)
print(A.Hikaf())
