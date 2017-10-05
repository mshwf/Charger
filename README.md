Charger
=======

What is Charger?
--------------
Charger is an object mapper, for easily mapping between properties in two objects.
The library has 4 methods as follows:
* ````ChargeFrom```` (2 overloads)
* ````Squeeze<TTarget>```` (2 overloads)

Get started
--------------
For explaining the use of Charger consider these two example classes:
```csharp
public class Item
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public string Category { get; set; }
    public string Url { get; set; }
    public Category Cat { get; set; }

}
public class ItemViewModel
{
    public int Id { get; set; }
    public string ItemName { get; set; }
    public string Category { get; set; }
    public string Url { get; set; }
    public DateTime DateUpdated { get; set; }
    public CategoryVM CatVm { get; set; }
}

````
* first case: you have an object of type ````ItemViewModel```` and you want to map its properties to an ````Item```` object,
so to do this, easily call the ````ChargeFrom```` extension method on the target object (````ItemViewModel```` object), like so:
```csharp
itemVm.ChargeFrom(item);
````
Similarily, you can do the same with lists, 
```csharp
itemsVm.ChargeFrom(items);
````

Note:
* Target's properties that are not exist (with the same name and type) on the source object will not change, so the ````DateUpdated````
will not touched by Charger.
* If the target list contains items, Charger will just add to them from the source.

Attributes
--------------
There are 3 attributes to use on the target properties:

* ````NotCharged````
Use this attribute on properties you don't want to charge.

* ````SourceProperty(propertyName)````
Use this property on target properties that have different names than the source property. for the ````ItemViewModel```` 
class you can charge the ````Id```` property from the ````ItemId```` on the ````Item```` class:

```csharp
[SourceProperty("ItemId")]
public int Id { get; set; }
````
It will throw a null reference exception if the proprty doesn't exist on the source object, to ignore the exception,
set the attribute's property ````AlwaysOnSource```` to ````false````:
```csharp
[SourceProperty("ItemId", AlwaysOnSource = false)]
public int Id { get; set; }
````
* ````DeepCharging````
So far, the charging is acheived for system properties that share the same type (int, string, bool, DateTime.. etc), 
but if you want to charge a custom property type from another custom property type, in the example above: 
````CatVm```` property from ````Cat````
you can use ````DeepCharging```` attribute, but since they have different names, you'll need to combine it with the ````SourceProperty````
attribute to specify the source's property name:
```csharp
[DeepCharging, SourceProperty("Cat")]
public CategoryVM CatVm { get; set; }
````
The ````DeepCharging```` uses ````ChargeFrom```` method internally, so the target property should be initialized (not null),
otherwise, it will throw a null reference exception, as a workaround you can initialize it using auto-properties:
```csharp
public CategoryVM CatVm { get; set; } = new CategoryVm();
````
But you can ignore charging it if it's ````null````, by setting ````NullTargetAction```` to ````NullTargetAction.IgnoreCharging````.

So far to use Charger you should have an initialized target object, either has a value or not..
But if all what you want is getting a fresh object out of another object, you can use ````Squeeze```` property that will just
get you a specified object type:

````csharp
var newItemModel = item.Squeeze<ItemViewModel>();  //for one object
var newItemsModel = items.Squeeze<ItemViewModel>(); //for list of object
````


