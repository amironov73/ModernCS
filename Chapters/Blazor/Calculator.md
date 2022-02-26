### Калькулятор

```html
@page "/calc"

<h1>Калькулятор</h1>

<div class="row">
    <input 
        type="number" 
        class="col-2"
        @bind="first" maxlength="5">
    <select
        class="col-2"
        @bind="operation">
        <option selected>+</option>
        <option>-</option>
        <option>*</option>
        <option>/</option>
    </select>
    <input 
        type="number"
        class="col-2"
        @bind="second" maxlength="5">
    =
    <input
        class="col-2"
        type="number" 
        @bind="sum" maxlength="5">
</div>
<div class="row">
    <button
        class="btn btn-primary col-8"
        @onclick="ComputeSum">
        Вычислить
    </button>
</div>

@code{
    private double first = 123;
    private string operation = "+";
    private double second = 345;
    private double sum;

    private void ComputeSum()
    {
        sum = operation switch 
            {
                "+" => first + second,
                "-" => first - second,
                "*" => first * second,
                "/" => first / second,
                _ => 0
            };
    }
}
```
