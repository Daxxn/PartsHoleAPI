# PartsHoleAPI

The API for managing all communication between the [Client](https://github.com/Daxxn/PartsHoleClient) and the database.

# Overview

The need for managing all the parts and tiny components needed for complex PCBs can get out of hand extremely quickly. It gets even worse when different parts have different packages and footprints. Other components like resistors and capacitors have hundreds of different values and sizes. Its alot.

## Features and Responibilities

- Serve the Client Data
- Databse Management
- Invoice Parsing
- BOM File Parsing
- Location (Bin) Management
- Custom Part Number Creation

### Future Plans

- Invoice reading directly from [DigiKey](https://www.digikey.com/) and [Mouaser](https://www.mouser.com/) APIs
- Order parts from [DigiKeys API](https://www.digikey.com/)and [Mouaser](https://www.mouser.com/)
- Update the suppliers API when custom part number changes are made (May not be possible, futher research is needed.)
- Parse KiCAD Project/Library files to access to more data (Change part numbers, Update supplier part numbers)

# Installation

## Prerequisites

The database is set up to use [MongoDB Atlas.](https://cloud.mongodb.com) Its the easiest to set up and deploy for me. That means a MongoDB database is required.

[Auth0](https://manage.auth0.com/dashboard) is used for authentication. Auth0 Client and API projects are required.

## Other Dependencies

- [CSV Parser Library - Daxxn](https://github.com/Daxxn/CSVParserLibrary-CS)
- [Excel Parser Library - Daxxn](https://github.com/Daxxn/ExcelParserLibrary)

