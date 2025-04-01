# MAUI Reproducability: Picker Problem
When using a picker and assigning it to a CollectionView-ed picker, it seems to remove values that are Observable. 

You can validate that it works correctly by moving the comment in the XAML page within the CollectionView from the Entry field to the Picker
(it would also work with a Label and perhaps more), and see that the tag remains. When the Picker is enabled, it removes the value 
from the field; watching the debugger, it is set to null after it is added to the ObservableCollection 
(MainPageViewModel.AddItem(), "Items.Add(i);"). 

Our guess as to why this happens is the Picker is enforcing a two-way binding, and overwriting the value inside the object with Null. 
We found this during a sorting test (hence the commented-out code for sorting) but it seems to happen when adding to a CollectionView
that is being controlled by a Picker. 

All relevant code snippets: 

## MainPage.xaml
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:ViewModels="clr-namespace:PickerProblem.MAUI.ViewModels"
             x:Class="PickerProblem.MAUI.MainPage">
  <VerticalStackLayout>
      <Picker ItemsSource="{Binding Tags}"
                  SelectedItem="{Binding NewTag}"
                  WidthRequest="100"
                  HeightRequest="40"
                  VerticalOptions="Center"/>

      <Button
          Command="{Binding AddItemCommand}"
          Text="Add Item"/>

      <!--<Button
          Command="{Binding SortItemsCommand}"
          Text="Sort"/>-->

      <CollectionView ItemsSource="{Binding Items}">
          <CollectionView.ItemTemplate>
              <DataTemplate>
                  <HorizontalStackLayout>
                      <Label Text="Item: " />
                      
                      <!--Move comment between Picker and Entry to see differences-->
                      <Picker ItemsSource="{Binding Source={RelativeSource AncestorType={x:Type ViewModels:MainPageViewModel}}, Path=Tags}"
                          SelectedItem="{Binding Tag}"
                          WidthRequest="100"
                          HeightRequest="40"
                          VerticalOptions="Center"/>
                      <!--
                      <Entry Text="{Binding Tag}"/>
                      -->
                      
                  </HorizontalStackLayout>
              </DataTemplate>
          </CollectionView.ItemTemplate>
      </CollectionView>
  </VerticalStackLayout>
</ContentPage>
``` 

## MainPageViewModel.cs
```csharp
public partial class MainPageViewModel : ObservableObject {
    [ObservableProperty]
    private ObservableCollection<Item> items;

    public List<string> Tags { get; } = new List<string> { "A", "B", "C", "D", "E" };

    [ObservableProperty]
    private string newTag = "";

    public MainPageViewModel() {
        items = new ObservableCollection<Item>();
    }

    [RelayCommand]
    private void AddItem() {
        Item i = new Item(NewTag);
        Items.Add(i);
        NewTag = "";
    }
}
```

## CSPROJ file (condensed)
This is left basically completely default except for the CommunityToolkit.Mvvm package, where we added ObservableObject, ObservableProperty, 
and RelayCommand attributes. 

```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFrameworks>net9.0-android;net9.0-ios;net9.0-maccatalyst</TargetFrameworks>
		<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net9.0-windows10.0.19041.0</TargetFrameworks>

		<OutputType>Exe</OutputType>
		<RootNamespace>PickerProblem.MAUI</RootNamespace>
		<UseMaui>true</UseMaui>
		<SingleProject>true</SingleProject>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<!-- Display name -->
		<ApplicationTitle>PickerProblem.MAUI</ApplicationTitle>

		<!-- App Identifier -->
		<ApplicationId>com.companyname.pickerproblem.maui</ApplicationId>

		<!-- Versions -->
		<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
		<ApplicationVersion>1</ApplicationVersion>

		<!-- To develop, package, and publish an app to the Microsoft Store, see: https://aka.ms/MauiTemplateUnpackaged -->
		<WindowsPackageType>None</WindowsPackageType>

		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
		<TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'tizen'">6.5</SupportedOSPlatformVersion>
	</PropertyGroup>

	<ItemGroup>
    ... Unchanged icon assets ...
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
		<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.0" />
	</ItemGroup>

</Project>

```