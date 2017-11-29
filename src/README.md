# sage50-windows-service
Windows Service that uses Sage50 SDK to integrate with Sage50 Desktop. Powers Sage50 Connector.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

You must install Sage50 Connector onto workstation where Sage50 Accountins System is installed.
Be sure that .Net Framework 4.5.2 is installed too.


### Installing

1. To install Sage50 Connector as Windows service you can use installutil.exe from .Net Framework or utilize install.bat file.
You should place install.bat into Sage50 Connector's folder, run Command Prompt as Administrator and go to Sage50 Connector's folder.
2. Rum install.bat
2. Run Services.msc and find Sage50 Connector serivce.
3. Open Properies menu and select 

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo


## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Actions supported

### Common Action data

All of Sage50 Actions should match with next schema:
```
{
  "$schema": "http://json-schema.org/draft-06/schema#",
  "title": "Sage Action JSON Schema",

  "type": "object",
  "properties": {
    "id": { "type": "string" },
    "type": {
      "type": "string",
      "enum": [
        "CreateInvoice",
        "UpdateInvoice",
        "UpsertCustomer",
        "CreatePayment"
      ]
    },
    "userId": { "type": "string" },
    "workflowId": { "type": "string" },
    "mainLogId": { "type": "string" },
    "createdAt": {
      "type": "string",
      "format": "date-time"
    },
    "source": { "type": "string" },
    "payload": { "type": "object" } // depends from "type" field of Action
  },
  "required": [
    "id",
    "type",
    "source"
  ]
}
```

**source** - should contains unique name of system from which the data came e.g.: ``Qualer``

**externalId** for Customer and Vendor contains unique Id from **source** system.

### UpsertCustomer

Create or update customer in Sage50.

Payload JSON structure:
```
{
  "$schema": "http://json-schema.org/draft-06/schema#",
  "title": "Upsert Customer Payload JSON Schema",

  "type": "object",
  "properties": {
    "companyName": { "type": "string" },
    "customer": { "$ref": "customer-schema.json" } 
  },
  "required": [
    "companyName",
    "customer"
  ]
}
```

### CreateInvoice / UpdateInvoice

Respectively Create or update Invoice in Sage50.

Payload JSON structure:
```
{
  "$schema": "http://json-schema.org/draft-06/schema#",
  "title": "Create/Update Invoice Payload JSON Schema",

  "type": "object",
  "properties": {
    "companyName": { "type": "string" },
    "invoice": { "$ref": "invoice-schema.json" } 
  },
  "required": [
    "companyName",
    "invoice"
  ]
}
```

### CreatePayment

Create new Payment in Sage50.

Payload JSON structure:
```
{
  "$schema": "http://json-schema.org/draft-06/schema#",
  "title": "Create Payload JSON Schema",

  "type": "object",
  "properties": {
    "companyName": { "type": "string" },
    "customer": { "$ref": "payment-schema.json" } 
  },
  "required": [
    "companyName",
    "payment"
  ]
}
```