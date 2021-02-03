# Pitstop Generated Documentation

## Service Architecture

![Pitstop solution architecture](https://github.com/EdwinVW/pitstop/wiki/img/solution-architecture.png)

## Commands

### Finish Maintenance Job

#### Services

- Workshop Management API
- Web App

#### Fields

|Property|Type|Description|
|-|-|-|
|EndTime|DateTime||
|JobId|Guid||
|MessageId|Guid||
|MessageType|string||
|Notes|string||
|StartTime|DateTime||

```plantuml
@startuml
skinparam SequenceMessageAlignment reverseDirection
skinparam SequenceGroupBodyBackgroundColor Transparent
skinparam SequenceBoxBackgroundColor #Gainsboro
skinparam SequenceArrowThickness 2
skinparam BoxPadding 10
skinparam ParticipantPadding 10
skinparam LifelineStrategy solid
hide footbox
participant "Web App" as WebApp
participant "Workshop Management API" as WorkshopManagementAPI
participant "Invoice Service" as InvoiceService
participant "Notification Service" as NotificationService
participant "Workshop Management Event Handler" as WorkshopManagementEventHandler
WebApp -[#DodgerBlue]> WorkshopManagementAPI : FinishMaintenanceJob
deactivate WebApp
activate WorkshopManagementAPI
||5||
group if [ModelState.IsValid]
WorkshopManagementAPI -[#ForestGreen]> InvoiceService : MaintenanceJobFinished
activate InvoiceService
WorkshopManagementAPI -[#ForestGreen]> NotificationService : MaintenanceJobFinished
activate NotificationService
WorkshopManagementAPI -[#ForestGreen]> WorkshopManagementEventHandler : MaintenanceJobFinished
activate WorkshopManagementEventHandler
deactivate InvoiceService
deactivate NotificationService
deactivate WorkshopManagementEventHandler
||5||
end
deactivate WorkshopManagementAPI
@enduml
```

### Plan Maintenance Job

#### Services

- Workshop Management API
- Web App

#### Fields

|Property|Type|Description|
|-|-|-|
|CustomerInfo|(string Id, string Name, string TelephoneNumber)||
|Description|string||
|EndTime|DateTime||
|JobId|Guid||
|MessageId|Guid||
|MessageType|string||
|StartTime|DateTime||
|VehicleInfo|(string LicenseNumber, string Brand, string Type)||

```plantuml
@startuml
skinparam SequenceMessageAlignment reverseDirection
skinparam SequenceGroupBodyBackgroundColor Transparent
skinparam SequenceBoxBackgroundColor #Gainsboro
skinparam SequenceArrowThickness 2
skinparam BoxPadding 10
skinparam ParticipantPadding 10
skinparam LifelineStrategy solid
hide footbox
participant "Web App" as WebApp
participant "Workshop Management API" as WorkshopManagementAPI
participant "Invoice Service" as InvoiceService
participant "Notification Service" as NotificationService
participant "Workshop Management Event Handler" as WorkshopManagementEventHandler
WebApp -[#DodgerBlue]> WorkshopManagementAPI : PlanMaintenanceJob
deactivate WebApp
activate WorkshopManagementAPI
||5||
group if [ModelState.IsValid]
WorkshopManagementAPI -[#ForestGreen]> InvoiceService : MaintenanceJobPlanned
activate InvoiceService
WorkshopManagementAPI -[#ForestGreen]> NotificationService : MaintenanceJobPlanned
activate NotificationService
WorkshopManagementAPI -[#ForestGreen]> WorkshopManagementEventHandler : MaintenanceJobPlanned
activate WorkshopManagementEventHandler
deactivate InvoiceService
deactivate NotificationService
deactivate WorkshopManagementEventHandler
||5||
end
deactivate WorkshopManagementAPI
@enduml
```

### Register Customer

#### Services

- Customer Management API
- Web App

#### Fields

|Property|Type|Description|
|-|-|-|
|Address|string||
|City|string||
|CustomerId|string||
|EmailAddress|string||
|MessageId|Guid||
|MessageType|string||
|Name|string||
|PostalCode|string||
|TelephoneNumber|string||

```plantuml
@startuml
skinparam SequenceMessageAlignment reverseDirection
skinparam SequenceGroupBodyBackgroundColor Transparent
skinparam SequenceBoxBackgroundColor #Gainsboro
skinparam SequenceArrowThickness 2
skinparam BoxPadding 10
skinparam ParticipantPadding 10
skinparam LifelineStrategy solid
hide footbox
participant "Web App" as WebApp
participant "Customer Management API" as CustomerManagementAPI
participant "Invoice Service" as InvoiceService
participant "Notification Service" as NotificationService
participant "Workshop Management Event Handler" as WorkshopManagementEventHandler
WebApp -[#DodgerBlue]> CustomerManagementAPI : RegisterCustomer
deactivate WebApp
activate CustomerManagementAPI
||5||
group if [ModelState.IsValid]
CustomerManagementAPI -[#ForestGreen]> InvoiceService : CustomerRegistered
activate InvoiceService
CustomerManagementAPI -[#ForestGreen]> NotificationService : CustomerRegistered
activate NotificationService
CustomerManagementAPI -[#ForestGreen]> WorkshopManagementEventHandler : CustomerRegistered
activate WorkshopManagementEventHandler
deactivate InvoiceService
deactivate NotificationService
deactivate WorkshopManagementEventHandler
||5||
end
deactivate CustomerManagementAPI
@enduml
```

### Register Planning

#### Services

- Web App
- Workshop Management API

#### Fields

|Property|Type|Description|
|-|-|-|
|MessageId|Guid||
|MessageType|string||
|PlanningDate|DateTime||

> ❗ No command handler found

### Register Vehicle

#### Services

- Vehicle Management
- Web App

#### Fields

|Property|Type|Description|
|-|-|-|
|Brand|string||
|LicenseNumber|string||
|MessageId|Guid||
|MessageType|string||
|OwnerId|string||
|Type|string||

```plantuml
@startuml
skinparam SequenceMessageAlignment reverseDirection
skinparam SequenceGroupBodyBackgroundColor Transparent
skinparam SequenceBoxBackgroundColor #Gainsboro
skinparam SequenceArrowThickness 2
skinparam BoxPadding 10
skinparam ParticipantPadding 10
skinparam LifelineStrategy solid
hide footbox
participant "Web App" as WebApp
participant "Vehicle Management" as VehicleManagement
participant "Workshop Management Event Handler" as WorkshopManagementEventHandler
WebApp -[#DodgerBlue]> VehicleManagement : RegisterVehicle
deactivate WebApp
activate VehicleManagement
||5||
group if [ModelState.IsValid]
VehicleManagement -[#ForestGreen]> WorkshopManagementEventHandler : VehicleRegistered
activate WorkshopManagementEventHandler
deactivate WorkshopManagementEventHandler
||5||
end
deactivate VehicleManagement
@enduml
```

## Events

### Customer Registered

#### Services

- Customer Management API
- Invoice Service
- Notification Service
- Workshop Management Event Handler

#### Fields

|Property|Type|Description|
|-|-|-|
|Address|string||
|City|string||
|CustomerId|string||
|EmailAddress|string||
|MessageId|Guid||
|MessageType|string||
|Name|string||
|PostalCode|string||
|TelephoneNumber|string||

### Day Has Passed

#### Services

- Invoice Service
- Notification Service
- Time Service

#### Fields

|Property|Type|Description|
|-|-|-|
|MessageId|Guid||
|MessageType|string||

### Maintenance Job Finished

#### Services

- Invoice Service
- Notification Service
- Workshop Management API
- Workshop Management Event Handler

#### Fields

|Property|Type|Description|
|-|-|-|
|EndTime|DateTime||
|JobId|string||
|JobId|Guid||
|MessageId|Guid||
|MessageType|string||
|Notes|string||
|StartTime|DateTime||

### Maintenance Job Planned

#### Services

- Invoice Service
- Notification Service
- Workshop Management API
- Workshop Management Event Handler

#### Fields

|Property|Type|Description|
|-|-|-|
|CustomerInfo|(string Id, string Name, string TelephoneNumber)||
|Description|string||
|EndTime|DateTime||
|JobId|string||
|JobId|Guid||
|MessageId|Guid||
|MessageType|string||
|StartTime|DateTime||
|VehicleInfo|(string LicenseNumber, string Brand, string Type)||

### Vehicle Registered

#### Services

- Vehicle Management
- Workshop Management Event Handler

#### Fields

|Property|Type|Description|
|-|-|-|
|Brand|string||
|LicenseNumber|string||
|MessageId|Guid||
|MessageType|string||
|OwnerId|string||
|Type|string||

### Workshop Planning Created

#### Services

- Workshop Management API

#### Fields

|Property|Type|Description|
|-|-|-|
|Date|DateTime||
|MessageId|Guid||
|MessageType|string||

## Aggregates

### Workshop Planning

```plantuml
@startuml
class "Workshop Planning Id" as WorkshopPlanningId <<(O,LightBlue)value object>> {
+Value: string
}
WorkshopPlanningId -- WorkshopPlanning
class "Workshop Planning" as WorkshopPlanning <<(R,LightGreen)root>> {
+Jobs: List<MaintenanceJob>
{static}+Create (Date)
+PlanMaintenanceJob (Command)
+FinishMaintenanceJob (Command)
}
class "Maintenance Job" as MaintenanceJob <<entity>> {
+Actual End Time: DateTime?
+Actual Start Time: DateTime?
+Customer: Customer
+Description: string
+End Time: DateTime
+Id: Guid
+Notes: string
+Start Time: DateTime
+Status: string
+Vehicle: Vehicle
+Plan (Description, Customer, Vehicle, End Time, Start Time, Id)
+Finish (Notes, Actual End Time, Actual Start Time)
}
class "Vehicle" as Vehicle <<entity>> {
+Brand: string
+Owner Id: string
+Type: string
}
MaintenanceJob -- Vehicle
class "Customer" as Customer <<entity>> {
+Name: string
+Telephone Number: string
}
MaintenanceJob -- Customer
WorkshopPlanning -- MaintenanceJob : 1..*
@enduml
```

