Hello and welcome to the 2025 journey of .NET MAUI!

In this article, we will explore how to build dynamic user interfaces with decision logic in .NET MAUI using XAML.

Dynamic UIs are essential for creating responsive and engaging applications that adapt to user input, application state, or external conditions. By leveraging data bindings, triggers, converters, and behaviors, you can implement dynamic UI elements that enhance the user experience and improve the efficiency of your .NET MAUI applications.

## Out of the box concepts for Dynamic UI in .NET MAUI

### 1. Data Binding
Data binding connects UI elements to data sources, enabling automatic updates when data changes. Use the `{Binding}` syntax to bind properties in XAML.

**Example:**
```xml
<Label Text="{Binding UserName}" FontSize="20" />
```

### 2. Triggers
Triggers let you change UI properties based on conditions.

**Example:**
```xml
<Entry Text="{Binding UserInput}">
    <Entry.Triggers>
        <DataTrigger TargetType="Entry" Binding="{Binding UserInput}" Value="">
            <Setter Property="Placeholder" Value="Please enter your name" />
        </DataTrigger>
    </Entry.Triggers>
</Entry>
```

### 3. Value Converters
Converters allow you to transform data from one format to another before displaying it.

**Example:**
```csharp
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Colors.Green : Colors.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

XAML Usage:
```xml
<Label Text="Status" TextColor="{Binding IsOnline, Converter={StaticResource BoolToColorConverter}}" />
```

### 4. Behaviors
Behaviors add reusable, event-driven functionality to controls.

**Example:**
```csharp
public class FocusBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Focused += OnFocused;
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.Focused -= OnFocused;
    }

    private void OnFocused(object sender, FocusEventArgs e)
    {
        ((Entry)sender).Text = "";
    }
}
```

XAML Usage:
```xml
<Entry Text="Enter text">
    <Entry.Behaviors>
        <local:FocusBehavior />
    </Entry.Behaviors>
</Entry>
```

But sometimes you want to add If and Switch conditions directly in your XAML for more dynamic UI capabilities. This makes it easier to handle conditional rendering without relying heavily on code-behind logic.

Let's create a `ConditionView` and `SwitchCaseView` that allow you to define If and Switch conditions in XAML.

## ConditionView

The `ConditionView` control renders its content based on a condition.

```csharp
public class ConditionView : ContentView
{
	public static readonly BindableProperty FalseProperty = BindableProperty.Create(nameof(False), typeof(View), typeof(ConditionView), default(View));
	public static readonly BindableProperty TrueProperty = BindableProperty.Create(nameof(True), typeof(View), typeof(ConditionView), default(View));
	public static readonly BindableProperty IfProperty = BindableProperty.Create(nameof(If), typeof(bool), typeof(ConditionView), default(bool), propertyChanged: ConditionChanged);

	private static void ConditionChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		var conditionView = (ConditionView)bindable;
		conditionView.Content = (bool)newvalue ? conditionView.True : conditionView.False;
	}

	public bool If
	{
		get => (bool)GetValue(IfProperty);
		set => SetValue(IfProperty, value);
	}

	public View True
	{
		get => (View)GetValue(TrueProperty);
		set => SetValue(TrueProperty, value);
	}

	public View False
	{
		get => (View)GetValue(FalseProperty);
		set => SetValue(FalseProperty, value);
	}
}
```

XAML Usage:

```xml
<mauiConditionView:ConditionView If="{Binding Condition}">
	<mauiConditionView:ConditionView.True>
		<Label Text="Condition is TRUE" />
	</mauiConditionView:ConditionView.True>
	<mauiConditionView:ConditionView.False>
		<Label Text="Condition is FALSE" />
	</mauiConditionView:ConditionView.False>
</mauiConditionView:ConditionView>
```

In this example, the rendered content will depend on Condition flag.

## Switch Case View

The `SwitchCaseView` control renders content based on multiple cases.

```csharp
public class SwitchCaseView<T> : ContentView
	where T : notnull
{
	public static readonly BindableProperty CasesProperty = BindableProperty.Create(nameof(Cases), typeof(ICollection<CaseView<T>>), typeof(ConditionView), new List<CaseView<T>>(), propertyChanged:SwitchChanged);
	public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(View), typeof(ConditionView), propertyChanged: SwitchChanged);
	public static readonly BindableProperty SwitchProperty = BindableProperty.Create(nameof(Switch), typeof(T), typeof(ConditionView), propertyChanged: SwitchChanged);

	private static void SwitchChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		var switchCaseView = (SwitchCaseView<T>)bindable;
		switchCaseView.Content = switchCaseView.Cases
		                                     .Where(x => x.Case.Equals(switchCaseView.Switch))
		                                     .Select(x => x.Content)
		                                     .SingleOrDefault(switchCaseView.Default);
	}

	public T Switch
	{
		get => (T)GetValue(SwitchProperty);
		set => SetValue(SwitchProperty, value);
	}

	public View? Default
	{
		get => (View?)GetValue(DefaultProperty);
		set => SetValue(DefaultProperty, value);
	}

	public ICollection<CaseView<T>> Cases
	{
		get => (ICollection<CaseView<T>>)GetValue(CasesProperty);
		set => SetValue(CasesProperty, value);
	}
}

public class CaseView<T> : ContentView
{
	public static readonly BindableProperty CaseProperty = BindableProperty.Create(nameof(Case), typeof(T), typeof(CaseView<T>));
	public T Case
	{
		get => (T)GetValue(CaseProperty);
		set => SetValue(CaseProperty, value);
	}
}
```

XAML Usage:

```xml
<mauiConditionView:SwitchCaseView x:TypeArguments="system:Int32" Switch="{Binding Case}">
	<mauiConditionView:SwitchCaseView.Cases>
		<mauiConditionView:CaseView x:TypeArguments="system:Int32" Case="1">
			<Label Text="Case 1" />
		</mauiConditionView:CaseView>
		<mauiConditionView:CaseView x:TypeArguments="system:Int32" Case="3">
			<Label Text="Case 3" />
		</mauiConditionView:CaseView>
	</mauiConditionView:SwitchCaseView.Cases>
	<mauiConditionView:SwitchCaseView.Default>
		<Label Text="Case Default" />
	</mauiConditionView:SwitchCaseView.Default>
</mauiConditionView:SwitchCaseView>
```

This `SwitchCaseView` will display a different Label depending on the Case value.

## Conclusion
Dynamic UI in .NET MAUI XAML empowers developers to create responsive, user-centric applications with minimal code. By combining data bindings, triggers, converters, behaviors, and controls like ConditionView and SwitchCaseView, you can handle complex UI requirements efficiently. Start building dynamic UIs today and enhance your app’s interactivity and usability!
