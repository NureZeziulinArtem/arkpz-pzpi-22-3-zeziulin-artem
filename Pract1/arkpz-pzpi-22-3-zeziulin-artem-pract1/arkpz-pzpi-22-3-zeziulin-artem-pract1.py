# Rule: Використовуйте 4 пробіли для кожного рівня відступу
# Bad code
if True:
	print("Відступ у 1 табуляцію")
# Fixed code
if True:
    print("Відступ у 4 пробіли")

# Rule: Зберігайте відступи при перенесенні частини коду на новий рядок
# Bad code
def my_function(arg1, arg2,
arg3, arg4):
    return arg1 + arg2 + arg3 + arg4
# Fixed code
def my_function(arg1, arg2,
                arg3, arg4):
    return arg1 + arg2 + arg3 + arg4

# Rule: Віддавайте перевагу пробілам над табуляцією
# Bad code
if True:
 	print("Змішані відступи табуляцією та пробілами")
# Fixed code
if True:
    print("Використано лише пробіли для відступу")

# Rule: Ліміт довжини строки – 79 символів
# Bad code
very_long_variable_name = "Це дуже довгий рядок, який значно перевищує рекомендований ліміт у 79 символів"
# Fixed code
very_long_variable_name = (
    "Це дуже довгий рядок, який розділено на кілька частин для зручності читання"
)

# Rule: Виконуйте перенесення перед бінарними операторами
# Bad code
result = 10 + 20 +
30
# Fixed code
result = 10 + 20 \
    + 30

# Rule: Використовуйте порожні рядки для логічного розділення коду
# Bad code
def function_one():
    pass
def function_two():
    pass
# Fixed code
def function_one():
    pass

def function_two():
    pass

# Rule: Дотримуйтеся правил форматування імпортів
# Bad code
import sys, os
from custom_package import *
# Fixed code
import os
import sys

from custom_package import specific_function

# Rule: Уникайте використання імпорту із символом зірочки (*)
# Bad code
from math import *
# Fixed code
from math import sqrt, pi

# Rule: Не перебільшуйте з використанням пробілів
# Bad code
my_list = [ 1, 2, 3 ]
result = ( 10 + 20 )
# Fixed code
my_list = [1, 2, 3]
result = (10 + 20)

# Rule: Використовуйте пробіли, коли вони потрібні
# Bad code
x=10+5*2
# Fixed code
x = 10 + 5*2

# Rule: Одна інструкція на рядок
# Bad code
x = 5; y = 10
# Fixed code
x = 5
y = 10

# Rule: Дотримуйтесь загальних правил написання коментарів
# Bad code
# Змінна x
x = 10
# Fixed code
# Змінна x відповідає за зберігання кількості елементів
x = 10

# Rule: Документуйте код за допомогою рядків документації (docstrings)
# Bad code
def calculate_area(radius):
    return 3.14 * radius ** 2

# Fixed code
def calculate_area(radius):
    """
    Calculate the area of a circle given its radius.

    Args:
        radius (float): The radius of the circle.

    Returns:
        float: The area of the circle.
    """
    return 3.14 * radius ** 2


# Rule: Використовуйте короткі lowercase назви модулів та пакетів
# Bad code
import MyCustomModule
# Fixed code
import my_custom_module

# Rule: Використовуйте PascalCase для назв класів
# Bad code
class my_class:
    pass
# Fixed code
class MyClass:
    pass

# Rule: Використовуйте lowercase для назв функцій та змінних
# Bad code
def MyFunction():
    pass
# Fixed code
def my_function():
    pass

# Rule: Використовуйте UPPERCASE для назв констант
# Bad code
pi = 3.14159
# Fixed code
PI = 3.14159

# Rule: Уникайте проблемні імена змінних
# Bad code
l = 10
# Fixed code
length = 10

# Rule: Використовуйте описові та зрозумілі імена змінних і функцій
# Bad code
def do_stuff(a, b):
    return a + b
# Fixed code
def calculate_total(price, tax):
    return price + tax

# Rule: Використовуйте is та is not для порівняння з сінглтонами
# Bad code
if value == None:
    pass
# Fixed code
if value is None:
    pass

# Rule: Використовуйте is not, а не not ... is
# Bad code
if not value is None:
    pass
# Fixed code
if value is not None:
    pass

# Rule: Завжди використовуйте def, а не присвоювання лямбда-виразу
# Bad code
add = lambda x, y: x + y
# Fixed code
def add(x, y):
    return x + y

# Rule: Уникайте використання змінюваних об’єктів як змінних за замовчуванням
# Bad code
def append_to_list(value, lst=[]):
    lst.append(value)
    return lst
# Fixed code
def append_to_list(value, lst=None):
    if lst is None:
        lst = []
    lst.append(value)
    return lst

# Rule: Використовуйте менеджери контексту для керування ресурсами
# Bad code
file = open('example.txt', 'r')
content = file.read()
file.close()
# Fixed code
with open('example.txt', 'r') as file:
    content = file.read()

# Rule: При обробці винятків, вказуйте конкретні типи винятків
# Bad code
try:
    x = 1 / 0
except:
    print("Error")
# Fixed code
try:
    x = 1 / 0
except ZeroDivisionError:
    print("Division by zero is not allowed")

# Rule: Функція або щось повертає, або нічого не повертає
# Bad code
def process_data(data):
    if not data:
        return
    return data.upper()
# Fixed code
def process_data(data):
    if not data:
        return None
    return data.upper()

# Rule: Для порівняння типів об'єктів використовуйте isinstance()
# Bad code
if type(x) == int:
    pass
# Fixed code
if isinstance(x, int):
    pass

# Rule: Не порівнюйте булеві значення з True або False
# Bad code
if is_ready == True:
    pass
# Fixed code
if is_ready:
    pass