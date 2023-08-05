# 函数指针与Lambda表达式

## 函数指针：

返回值类型(*函数名)(参数类型)

如：`void(*func)(int)`，可以作为参数传入，在其他函数中调用

例：

```c++
void ForEach(cosnt std::vector<int>& values, void(*func)(int))

{
    for(int value:values)
        func(value)
}

int main()
{
    std::vector<int> values{2,4,3,4,1,5,5,1};
    ForEach(values,[](int value){std::cout<<"Values:"<<value<<std::endl;});
    
}
```

这里用到了C++11的特性，基于范围的for循环

## Lambda表达式：

```c++
auto lambda = [](int value){std::cout<<"Values:"<<value<<std::endl;};
ForEach(values,lambda);
```



### Lambda表达式的写法：

#### `[captures](params) return-type {body}`

↑↓二者的区别在于有无返回值类型声明，一般可以省略掉，让编译器来推导

#### `[captures](params){body}`



tips:在C++20和C++23还有另外两种写法:

#### `[captures]<tparams>(params) lambda-specfiers {}`

↑C++20引入，让lambda可以像模板函数一样被调用

#### `[capture] lambda-specifiers {}`

```c++
int a =5
auto lambda = [=](params){a=7;DoSomeThing();};
```

这时会报错，因为按值传递捕获，我们不能在函数内部修改外部变量

`auto lambda = [=](params)mutable{a=7;DoSomeThing();};`

可以加上lambda-specifiers <font size=5>`mutable`</font>,就不会报错了，但是因为是值传递，所以lambda表达式内部对外部变量的修改不会影响外部变量

##### 在captures中可以捕获Lambda表达式外部的变量

例：

```c++
int main
{
	int a=5;
	int b=6;
	int c=7;
	auto lambda_1 = [=](int value){};
    auto lambda_2 = [&](int value){};
    auto lambda_3 = [a](int value){};
    auto lambda_4 = [&b](int value){};
}
```

这四种写法分别是：

##### [=]以值传递捕获所有变量(a、b、c)

##### [&]以引用传递捕获所有变量

##### [a]以值传递捕获a

##### [&a]以引用传递捕获b

捕获列表中可以捕获多个变量，用逗号隔开

##### [=,&a]除a用引用传递外，其余变量都用值传递，还可以按引用捕获多个[=,&a,&b,&c]，a、b、c必须按引用捕获

##### [&,a]除a用值传递外，其余变量都用引用传递，还可以按值传递捕获多个[&,a,b,c]，a、b、c必须按值传递捕获

##### [this]还可以捕获this指针，[=]和[&]也会捕获this指针

被捕获的外部变量就可以在函数内部调用了

```c++
void ForEach(cosnt std::vector<int>& values, void(*func)(int))

{
    for(int value:values)
        func(value)
}

int main()
{
	int a = 5;
	std::vector<int> values = {1, 3, 5, 7, 9, 2, 4, 3, 4, 0, 7, 2, 1};
	auto lambda = [=](int value) {std::cout << "Value:" << value << std::endl; };
	ForEach(values, lambda);
}
```

当我们加上捕获时，就出错了，因为ForEach传入的函数指针是原始函数指针

```c++
#include<functional>

void ForEach(const std::vector<int>& values, const std::function<void(int)>& func)
{
    
}
```

改成使用std::function就可以了
