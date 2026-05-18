# 📐 XAML REFERENCE: DashboardPage - Implementación Completa

## Estructura Recomendada para tu DashboardPage.xaml

Este documento te proporciona el código XAML que necesitas para que funcione el nuevo `DashboardViewModel` con cambio dinámico de doctores y búsqueda de pacientes.

---

## Sección 1: Selector de Doctor

```xaml
<!-- Seleccionar un doctor de la lista -->
<Frame BorderColor="#E0E0E0" CornerRadius="10" Padding="15">
    <VerticalStackLayout Spacing="10">
        <Label Text="Selecciona un Doctor"
               FontSize="16"
               FontAttributes="Bold" />

        <!-- Picker para elegir doctor -->
        <Picker ItemsSource="{Binding DoctoresDisponibles}"
                SelectedItem="{Binding MedicoActual}"
                ItemDisplayBinding="{Binding NombreCompleto}"
                Title="Elige un doctor"
                BackgroundColor="White" />

        <!-- Mostrar info del doctor seleccionado -->
        <StackLayout IsVisible="{Binding MedicoActual, Converter={StaticResource NotNullConverter}}"
                     Spacing="5">
            <Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto,Auto">
                <Label Text="Especialidad:" FontAttributes="Bold" Grid.Row="0" />
                <Label Text="{Binding MedicoActual.Especialidad}" Grid.Row="0" Grid.Column="1" />

                <Label Text="Consultorio:" FontAttributes="Bold" Grid.Row="1" />
                <Label Text="{Binding MedicoActual.Consultorio}" Grid.Row="1" Grid.Column="1" />

                <Label Text="Email:" FontAttributes="Bold" Grid.Row="2" />
                <Label Text="{Binding MedicoActual.Email}" Grid.Row="2" Grid.Column="1" TextColor="Blue" />
            </Grid>
        </StackLayout>
    </VerticalStackLayout>
</Frame>
```

---

## Sección 2: Estadísticas de Citas

```xaml
<!-- Mostrar estadísticas en 3 columnas -->
<Frame BorderColor="#2196F3" CornerRadius="10" Padding="15">
    <Grid ColumnDefinitions="*,*,*" Spacing="10">
        <!-- Total Citas -->
        <VerticalStackLayout Spacing="5" Grid.Column="0">
            <Label Text="{Binding TotalCitasHoy}"
                   FontSize="32"
                   FontAttributes="Bold"
                   TextColor="#2196F3"
                   HorizontalTextAlignment="Center" />
            <Label Text="Total Citas"
                   FontSize="12"
                   TextColor="Gray"
                   HorizontalTextAlignment="Center" />
        </VerticalStackLayout>

        <!-- Citas Confirmadas -->
        <VerticalStackLayout Spacing="5" Grid.Column="1">
            <Label Text="{Binding CitasConfirmadas}"
                   FontSize="32"
                   FontAttributes="Bold"
                   TextColor="#4CAF50"
                   HorizontalTextAlignment="Center" />
            <Label Text="Confirmadas"
                   FontSize="12"
                   TextColor="Gray"
                   HorizontalTextAlignment="Center" />
        </VerticalStackLayout>

        <!-- Citas Pendientes -->
        <VerticalStackLayout Spacing="5" Grid.Column="2">
            <Label Text="{Binding CitasPendientes}"
                   FontSize="32"
                   FontAttributes="Bold"
                   TextColor="#FF9800"
                   HorizontalTextAlignment="Center" />
            <Label Text="Pendientes"
                   FontSize="12"
                   TextColor="Gray"
                   HorizontalTextAlignment="Center" />
        </VerticalStackLayout>
    </Grid>
</Frame>
```

---

## Sección 3: Búsqueda de Paciente por Cédula ⭐

```xaml
<!-- ESTE ES EL NUEVO: Búsqueda de paciente y asignación de email -->
<Frame BorderColor="#E0E0E0" CornerRadius="10" Padding="15">
    <VerticalStackLayout Spacing="10">
        <Label Text="Buscar Paciente por Cédula"
               FontSize="16"
               FontAttributes="Bold" />

        <!-- Campo de entrada + Botón de búsqueda -->
        <Grid ColumnDefinitions="*,Auto" Spacing="10">
            <Entry Placeholder="Ingresa la cédula del paciente"
                   Text="{Binding BusquedaCedula}"
                   Keyboard="Numeric"
                   BackgroundColor="White"
                   Grid.Column="0" />

            <Button Text="🔍 Buscar"
                    Command="{Binding BuscarPacientePorCedulaCommand}"
                    BackgroundColor="#2196F3"
                    TextColor="White"
                    Grid.Column="1" />
        </Grid>

        <!-- Mensaje de búsqueda -->
        <Label Text="{Binding MensajeBusqueda}"
               IsVisible="{Binding MensajeBusqueda, Converter={StaticResource NotNullConverter}}"
               FontSize="12"
               TextColor="Gray" />

        <!-- DATOS DEL PACIENTE ENCONTRADO (Data Binding Automático) -->
        <StackLayout IsVisible="{Binding PacienteBuscado, Converter={StaticResource NotNullConverter}}"
                     Padding="10"
                     BackgroundColor="#E8F5E9"
                     CornerRadius="8">

            <Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto,Auto,Auto" Spacing="5">
                <!-- Nombre -->
                <Label Text="Nombre:"
                       FontAttributes="Bold"
                       Grid.Row="0" />
                <Label Text="{Binding PacienteBuscado.NombreCompleto}"
                       Grid.Row="0"
                       Grid.Column="1" />

                <!-- Cédula -->
                <Label Text="Cédula:"
                       FontAttributes="Bold"
                       Grid.Row="1" />
                <Label Text="{Binding PacienteBuscado.Cedula}"
                       Grid.Row="1"
                       Grid.Column="1" />

                <!-- EMAIL (EL QUE SOLICITASTE) ⭐ -->
                <Label Text="Email:"
                       FontAttributes="Bold"
                       Grid.Row="2" />
                <Label Text="{Binding PacienteBuscado.Email}"
                       Grid.Row="2"
                       Grid.Column="1"
                       TextColor="#1976D2"
                       FontAttributes="Bold" />

                <!-- Teléfono -->
                <Label Text="Teléfono:"
                       FontAttributes="Bold"
                       Grid.Row="3" />
                <Label Text="{Binding PacienteBuscado.Telefono}"
                       Grid.Row="3"
                       Grid.Column="1" />
            </Grid>

            <!-- Botón para limpiar -->
            <Button Text="Limpiar Búsqueda"
                    Command="{Binding LimpiarBusquedaCommand}"
                    BackgroundColor="#F44336"
                    TextColor="White"
                    Margin="0,10,0,0" />
        </StackLayout>
    </VerticalStackLayout>
</Frame>
```

---

## Sección 4: Citas de Hoy

```xaml
<!-- Mostrar citas de hoy del doctor seleccionado -->
<Frame BorderColor="#E0E0E0" CornerRadius="10" Padding="15">
    <VerticalStackLayout Spacing="10">
        <Label Text="Citas de Hoy"
               FontSize="16"
               FontAttributes="Bold" />

        <!-- Indicador de carga -->
        <ActivityIndicator IsRunning="{Binding IsBusy}"
                         IsVisible="{Binding IsBusy}"
                         Color="#2196F3" />

        <!-- Lista de citas -->
        <CollectionView ItemsSource="{Binding CitasHoy}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame BorderColor="#4CAF50"
                           CornerRadius="8"
                           Padding="12"
                           Margin="0,5">
                        <VerticalStackLayout Spacing="5">
                            <Label Text="{Binding Paciente.NombreCompleto}"
                                   FontSize="14"
                                   FontAttributes="Bold" />

                            <Label Text="{Binding Motivo, StringFormat='Motivo: {0}'}"
                                   FontSize="12"
                                   TextColor="Gray" />

                            <Label Text="{Binding FechaHora, StringFormat='Hora: {0:HH:mm}'}"
                                   FontSize="12"
                                   TextColor="Gray" />

                            <Label Text="{Binding DuracionMinutos, StringFormat='Duración: {0} min'}"
                                   FontSize="11"
                                   TextColor="Gray" />

                            <Label Text="{Binding Estado}"
                                   FontSize="12"
                                   FontAttributes="Bold"
                                   TextColor="#4CAF50" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- Mensaje cuando no hay citas -->
        <Label Text="No hay citas programadas para hoy"
               FontSize="12"
               TextColor="Gray"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding CitasHoy, Converter={StaticResource IsNullOrEmptyConverter}}" />
    </VerticalStackLayout>
</Frame>
```

---

## Sección 5: Citas Próximas Pendientes

```xaml
<!-- Citas futuras que no están confirmadas -->
<Frame BorderColor="#E0E0E0" CornerRadius="10" Padding="15">
    <VerticalStackLayout Spacing="10">
        <Label Text="Citas Próximas Pendientes de Confirmar"
               FontSize="16"
               FontAttributes="Bold" />

        <CollectionView ItemsSource="{Binding CitasProximas}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame BorderColor="#FF9800"
                           CornerRadius="8"
                           Padding="12"
                           Margin="0,5">
                        <VerticalStackLayout Spacing="5">
                            <Label Text="{Binding Paciente.NombreCompleto}"
                                   FontSize="14"
                                   FontAttributes="Bold" />

                            <Label Text="{Binding Motivo, StringFormat='Motivo: {0}'}"
                                   FontSize="12"
                                   TextColor="Gray" />

                            <Label Text="{Binding FechaHora, StringFormat='Fecha: {0:dd/MM/yyyy HH:mm}'}"
                                   FontSize="12"
                                   TextColor="Gray" />

                            <Label Text="⚠️ Pendiente de Confirmación"
                                   FontSize="12"
                                   FontAttributes="Bold"
                                   TextColor="#FF9800" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <Label Text="No hay citas próximas pendientes"
               FontSize="12"
               TextColor="Gray"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding CitasProximas, Converter={StaticResource IsNullOrEmptyConverter}}" />
    </VerticalStackLayout>
</Frame>
```

---

## Nota Importante: Converters

Para que funcionen los `Converter` en el XAML, necesitas agregar estas líneas al inicio de tu `App.xaml`:

```xaml
<Application.Resources>
    <ResourceDictionary>
        <!-- Converters -->
        <converters:NotNullConverter x:Key="NotNullConverter" />
        <converters:IsNullOrEmptyConverter x:Key="IsNullOrEmptyConverter" />
    </ResourceDictionary>
</Application.Resources>
```

---

## Estructura Completa DashboardPage.xaml

Si quieres una página completa, copia esto en tu `DashboardPage.xaml`:

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="DoctorApp.Views.DashboardPage"
             Title="Panel de Control"
             BackgroundColor="#F5F5F5">

    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="20">

            <!-- [INSERTA LA SECCIÓN 1: Selector de Doctor] -->

            <!-- [INSERTA LA SECCIÓN 2: Estadísticas] -->

            <!-- [INSERTA LA SECCIÓN 3: Búsqueda de Paciente] ⭐ IMPORTANTE -->

            <!-- [INSERTA LA SECCIÓN 4: Citas de Hoy] -->

            <!-- [INSERTA LA SECCIÓN 5: Citas Próximas] -->

            <!-- Botón Refrescar -->
            <Button Text="🔄 Refrescar"
                    Command="{Binding RefrescarCommand}"
                    BackgroundColor="#2196F3"
                    TextColor="White"
                    FontSize="14"
                    Padding="15" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

---

## Binding Cheatsheet

| Binding | Tipo | Descripción |
|---------|------|-------------|
| `{Binding DoctoresDisponibles}` | ObservableCollection<Medico> | Lista de 3 doctores |
| `{Binding MedicoActual}` | Medico | Doctor seleccionado |
| `{Binding MedicoActual.NombreCompleto}` | string | Nombre completo del doctor |
| `{Binding CitasHoy}` | ObservableCollection<Cita> | Citas de hoy |
| `{Binding CitasProximas}` | ObservableCollection<Cita> | Citas futuras pendientes |
| `{Binding TotalCitasHoy}` | int | Número total de citas |
| `{Binding CitasConfirmadas}` | int | Citas confirmadas |
| `{Binding CitasPendientes}` | int | Citas sin confirmar |
| `{Binding BusquedaCedula}` | string | Campo de entrada de cédula |
| `{Binding PacienteBuscado}` | Paciente? | Paciente encontrado |
| `{Binding PacienteBuscado.Email}` | string | ⭐ **EMAIL DEL PACIENTE** |
| `{Binding MensajeBusqueda}` | string | Mensaje de estado búsqueda |
| `{Binding SeleccionarDoctorCommand}` | ICommand | Cambiar doctor |
| `{Binding BuscarPacientePorCedulaCommand}` | ICommand | Buscar paciente |
| `{Binding LimpiarBusquedaCommand}` | ICommand | Limpiar búsqueda |

---

## Datos Disponibles en Cada Cita

Cuando bindeas a `CitasHoy` o `CitasProximas`, cada `Cita` tiene:

```csharp
Cita {
    Id: int,
    FechaHora: DateTime,
    Motivo: string,
    Confirmada: bool,
    Asistio: bool,
    DuracionMinutos: int,
    Estado: EstadoCita,
    Paciente: Paciente {
        Id: int,
        Nombre: string,
        Apellido: string,
        Cedula: string,
        Email: string,
        Telefono: string
    }
}
```

---

## Ejemplo de Uso Completo

```xaml
<!-- Mostrar nombre del paciente de la cita -->
<Label Text="{Binding Paciente.NombreCompleto}" />

<!-- Mostrar hora formateada -->
<Label Text="{Binding FechaHora, StringFormat='Hora: {0:HH:mm}'}" />

<!-- Mostrar si está confirmada -->
<Label Text="{Binding Estado}" />

<!-- Mostrar email del paciente -->
<Label Text="{Binding Paciente.Email}" />
```

---

## Tips de Styling

### Colores recomendados
```csharp
#2196F3  // Azul (principal)
#4CAF50  // Verde (confirmado)
#FF9800  // Naranja (pendiente)
#F44336  // Rojo (cancelar)
#FFC107  // Amarillo (advertencia)
```

### Tamaños recomendados
```csharp
Títulos: FontSize="18" FontAttributes="Bold"
Subtítulos: FontSize="14" FontAttributes="Bold"
Datos: FontSize="12"
Etiquetas: FontSize="11" TextColor="Gray"
```

---

## ✅ Checklist de Implementación XAML

- [ ] Agregaste Sección 1: Selector de Doctor
- [ ] Agregaste Sección 2: Estadísticas (Total, Confirmadas, Pendientes)
- [ ] Agregaste Sección 3: Búsqueda de Paciente (⭐ Email binding)
- [ ] Agregaste Sección 4: Citas de Hoy
- [ ] Agregaste Sección 5: Citas Próximas
- [ ] Registraste los Converters en App.xaml
- [ ] Compiló sin errores

---

## Próximos Pasos

1. Copia las secciones en tu `DashboardPage.xaml`
2. Actualiza los colores si lo deseas
3. Agrega fotos de los doctores (Image control)
4. Agrega animaciones al cambiar doctor
5. Personaliza según tu diseño

¡Listo para funcionar! 🚀
