# Receipt Processor Microservice

This project implements a receipt processing microservice in C# .NET 8. It provides two main API endpoints for processing receipts and retrieving awarded points based on a set of predefined rules.

## Project Structure

```
.
├── ReceiptProcessor                     # Source files
│   ├── Controller
│   │   └── ReceiptProcessorController.cs
│   ├── Models
│   │   ├── GetPoints.cs
│   │   ├── ProcessReceiptsInput.cs
│   │   └── ProcessReceiptsOutput.cs
│   ├── appsettings.json
│   ├── Dockerfile
│   ├── Program.cs
│   └── ReceiptProcessor.csproj
├── ReceiptProcessor.Tests               # Automated tests
│   ├── ReceiptProcessor.Tests.csproj
│   └── ReceiptProcessorTests.cs
├── ReceiptProcessorChallenge.sln
├── ReceiptProcessorChallenge.postman_collection.json
└── README.md
```

## Project Description

The Receipt Processor microservice provides an API to process receipts and retrieve the awarded points based on the receipt's details. Points are calculated by various rules specified in the challenge description.

## Setup and Running Instructions

### Prerequisites
- **Docker** (recommended for isolated setup)
- **.NET SDK** (if running locally without Docker)

### Running with Docker

1. Navigate to the source directory:
   ```bash
   cd ReceiptProcessor
   ```
2. Build the Docker image:
   ```bash
   docker build -t receipt-processor .
   ```
3. Run the Docker container:
   ```bash
   docker run -p 8080:8080 receipt-processor
   ```
The service will be available at `http://localhost:8080`.

### Running Locally

1. Navigate to the development directory:
   ```bash
   cd ReceiptProcessor
   ```
2. Install dependencies:
   ```bash
   dotnet restore
   ```
3. Run the service:
   ```bash
   dotnet run --project src/ReceiptProcessor
   ```
Where the service is available can be found on the logs once it is started.


## Testing

This project uses **xUnit** for testing. Tests can be run with the following command on the root directory:

```bash
dotnet test
```

### Postman Collection

A Postman collection is attached to help with local and Dockerized testing of the API endpoints. You can import this collection into Postman to quickly set up and test the receipt processing and points retrieval functions.

### Notes on Testing

- **Testing Framework**: xUnit was chosen for testing purposes. 
- **Test Ordering**: Due to the requirement to ensure correct `id` generation and to avoid test ordering, an internal counter was implemented.
- **Invalid Input Testing**: Due to limitations with producing invalid input for testing purporses, Process Receipts Endpoint was tested manually.
- **Integration Testing**: As this is a single-component service, it was determined that integration testing was not required.

## API Endpoints

### 1. Process Receipts
- **Path**: `/receipts/process`
- **Method**: `POST`
- **Payload**: JSON representing the receipt
- **Response**: JSON containing an `id` for the receipt.

Example Response:
```json
{ "id": "7fb1377b-b223-49d9-a31a-5a02701dd310" }
```

### 2. Get Points
- **Path**: `/receipts/{id}/points`
- **Method**: `GET`
- **Response**: JSON containing the number of points awarded.

Example Response:
```json
{ "points": 32 }
```

## Rules for Calculating Points

- One point for every alphanumeric character in the retailer name.
- 50 points if the total is a round dollar amount with no cents.
- 25 points if the total is a multiple of `0.25`.
- 5 points for every two items on the receipt.
- If the trimmed length of the item description is a multiple of 3, multiply the price by `0.2` and round up to the nearest integer. The result is the number of points earned.
- 6 points if the day in the purchase date is odd.
- 10 points if the time of purchase is between 2:00 PM and 4:00 PM.

## Example

```json
{
  "retailer": "Target",
  "purchaseDate": "2022-01-01",
  "purchaseTime": "13:01",
  "items": [
    { "shortDescription": "Mountain Dew 12PK", "price": "6.49" },
    { "shortDescription": "Emils Cheese Pizza", "price": "12.25" },
    { "shortDescription": "Knorr Creamy Chicken", "price": "1.26" },
    { "shortDescription": "Doritos Nacho Cheese", "price": "3.35" },
    { "shortDescription": "   Klarbrunn 12-PK 12 FL OZ  ", "price": "12.00" }
  ],
  "total": "35.35"
}
```

Total Points: 28  
Breakdown:
- 6 points - retailer name has 6 characters
- 10 points - 4 items (2 pairs @ 5 points each)
- 3 points - "Emils Cheese Pizza" is 18 characters (a multiple of 3), item price of 12.25 * 0.2 = 2.45, rounded up is 3 points
- 3 points - "Klarbrunn 12-PK 12 FL OZ" is 24 characters (a multiple of 3), item price of 12.00 * 0.2 = 2.4, rounded up is 3 points
- 6 points - purchase day is odd

Total = 28 points
