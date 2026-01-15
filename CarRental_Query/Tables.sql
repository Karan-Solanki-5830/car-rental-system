-- Table: Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Password VARCHAR(10) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL
);

-- Table: User
CREATE TABLE [User] (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Password VARCHAR(10) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Role VARCHAR(10) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL
);

-- Table: VehicleType
CREATE TABLE VehicleType (
    VehicleTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName VARCHAR(50) NOT NULL,
    UserID INT NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);

-- Table: FuelType
CREATE TABLE FuelType (
    FuelTypeID INT PRIMARY KEY IDENTITY(1,1),
    FuelName VARCHAR(50) NOT NULL,
    UserID INT NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);

-- Table: Vehicle
CREATE TABLE Vehicle (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Brand VARCHAR(50) NOT NULL,
    Model VARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    PlateNumber VARCHAR(20) NOT NULL,
    FuelTypeID INT NOT NULL,
    VehicleTypeID INT NOT NULL,
    ImageURL VARCHAR(MAX) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Mileage FLOAT NOT NULL,
    ConditionNote TEXT NOT NULL,
    PricePerHour DECIMAL(10,2) NOT NULL,
    PricePerDay DECIMAL(10,2) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (FuelTypeID) REFERENCES FuelType (FuelTypeID),
    FOREIGN KEY (VehicleTypeID) REFERENCES VehicleType (VehicleTypeID),
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);

-- Table: Booking
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    CustomerID INT NOT NULL,
    VehicleID INT NOT NULL,
    StartDateTime DATETIME NOT NULL,
    EndDateTime DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID),
    FOREIGN KEY (UserID) REFERENCES [User] (UserID),
    FOREIGN KEY (VehicleID) REFERENCES Vehicle (VehicleID)
);

-- Table: Agreement
CREATE TABLE Agreement (
    AgreementID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT NOT NULL,
    UserID INT NOT NULL,
    TermsAccepted BIT DEFAULT 0,
    AgreementDate DATETIME NOT NULL,
    AgreementPDFPath VARCHAR(MAX) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (BookingID) REFERENCES Booking (BookingID),
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);

-- Table: Payment
CREATE TABLE Payment (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT NOT NULL,
    UserID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    PaymentMethod VARCHAR(50) NOT NULL,
    Remarks TEXT NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (BookingID) REFERENCES Booking (BookingID),
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);

-- Table: MaintenanceLog
CREATE TABLE MaintenanceLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    VehicleID INT NOT NULL,
    UserID INT NOT NULL,
    Description TEXT NOT NULL,
    ServiceDate DATETIME NOT NULL,
    Cost DECIMAL(10,2) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    FOREIGN KEY (VehicleID) REFERENCES Vehicle (VehicleID),
    FOREIGN KEY (UserID) REFERENCES [User] (UserID)
);


