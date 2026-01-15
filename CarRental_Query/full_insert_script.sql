-- Insert into Customer
INSERT INTO Customer (FullName, Email, Password, Phone, Modified)
VALUES
('Customer1', 'customer1@mail.com', 'pass001', '9000000001', '2025-07-03 02:32:30'),
('Customer2', 'customer2@mail.com', 'pass002', '9000000002', '2025-07-03 02:32:30'),
('Customer3', 'customer3@mail.com', 'pass003', '9000000003', '2025-07-03 02:32:30'),
('Customer4', 'customer4@mail.com', 'pass004', '9000000004', '2025-07-03 02:32:30'),
('Customer5', 'customer5@mail.com', 'pass005', '9000000005', '2025-07-03 02:32:30'),
('Customer6', 'customer6@mail.com', 'pass006', '9000000006', '2025-07-03 02:32:30'),
('Customer7', 'customer7@mail.com', 'pass007', '9000000007', '2025-07-03 02:32:30'),
('Customer8', 'customer8@mail.com', 'pass008', '9000000008', '2025-07-03 02:32:30'),
('Customer9', 'customer9@mail.com', 'pass009', '9000000009', '2025-07-03 02:32:30'),
('Customer10', 'customer10@mail.com', 'pass010', '9000000010', '2025-07-03 02:32:30'),
('Customer11', 'customer11@mail.com', 'pass011', '9000000011', '2025-07-03 02:32:30'),
('Customer12', 'customer12@mail.com', 'pass012', '9000000012', '2025-07-03 02:32:30'),
('Customer13', 'customer13@mail.com', 'pass013', '9000000013', '2025-07-03 02:32:30'),
('Customer14', 'customer14@mail.com', 'pass014', '9000000014', '2025-07-03 02:32:30'),
('Customer15', 'customer15@mail.com', 'pass015', '9000000015', '2025-07-03 02:32:30');

-- Insert into [User]
INSERT INTO [User] (Name, Email, Password, Phone, Role, Modified)
VALUES
('User1', 'user1@mail.com', 'pwd001', '8000000001', 'Admin', '2025-07-03 02:32:30'),
('User2', 'user2@mail.com', 'pwd002', '8000000002', 'Admin', '2025-07-03 02:32:30'),
('User3', 'user3@mail.com', 'pwd003', '8000000003', 'Admin', '2025-07-03 02:32:30'),
('User4', 'user4@mail.com', 'pwd004', '8000000004', 'Admin', '2025-07-03 02:32:30'),
('User5', 'user5@mail.com', 'pwd005', '8000000005', 'Admin', '2025-07-03 02:32:30'),
('User6', 'user6@mail.com', 'pwd006', '8000000006', 'Admin', '2025-07-03 02:32:30'),
('User7', 'user7@mail.com', 'pwd007', '8000000007', 'Admin', '2025-07-03 02:32:30'),
('User8', 'user8@mail.com', 'pwd008', '8000000008', 'Admin', '2025-07-03 02:32:30'),
('User9', 'user9@mail.com', 'pwd009', '8000000009', 'Admin', '2025-07-03 02:32:30'),
('User10', 'user10@mail.com', 'pwd010', '8000000010', 'Admin', '2025-07-03 02:32:30'),
('User11', 'user11@mail.com', 'pwd011', '8000000011', 'Admin', '2025-07-03 02:32:30'),
('User12', 'user12@mail.com', 'pwd012', '8000000012', 'Admin', '2025-07-03 02:32:30'),
('User13', 'user13@mail.com', 'pwd013', '8000000013', 'Admin', '2025-07-03 02:32:30'),
('User14', 'user14@mail.com', 'pwd014', '8000000014', 'Admin', '2025-07-03 02:32:30'),
('User15', 'user15@mail.com', 'pwd015', '8000000015', 'Admin', '2025-07-03 02:32:30');

-- Insert into VehicleType
INSERT INTO VehicleType (TypeName, UserID, Modified)
VALUES
('Type1', 1, '2025-07-03 02:32:30'),
('Type2', 2, '2025-07-03 02:32:30'),
('Type3', 3, '2025-07-03 02:32:30'),
('Type4', 4, '2025-07-03 02:32:30'),
('Type5', 5, '2025-07-03 02:32:30'),
('Type6', 6, '2025-07-03 02:32:30'),
('Type7', 7, '2025-07-03 02:32:30'),
('Type8', 8, '2025-07-03 02:32:30'),
('Type9', 9, '2025-07-03 02:32:30'),
('Type10', 10, '2025-07-03 02:32:30'),
('Type11', 11, '2025-07-03 02:32:30'),
('Type12', 12, '2025-07-03 02:32:30'),
('Type13', 13, '2025-07-03 02:32:30'),
('Type14', 14, '2025-07-03 02:32:30'),
('Type15', 15, '2025-07-03 02:32:30');

-- Insert into FuelType
INSERT INTO FuelType (FuelName, UserID, Modified)
VALUES
('Fuel1', 1, '2025-07-03 02:32:30'),
('Fuel2', 2, '2025-07-03 02:32:30'),
('Fuel3', 3, '2025-07-03 02:32:30'),
('Fuel4', 4, '2025-07-03 02:32:30'),
('Fuel5', 5, '2025-07-03 02:32:30'),
('Fuel6', 6, '2025-07-03 02:32:30'),
('Fuel7', 7, '2025-07-03 02:32:30'),
('Fuel8', 8, '2025-07-03 02:32:30'),
('Fuel9', 9, '2025-07-03 02:32:30'),
('Fuel10', 10, '2025-07-03 02:32:30'),
('Fuel11', 11, '2025-07-03 02:32:30'),
('Fuel12', 12, '2025-07-03 02:32:30'),
('Fuel13', 13, '2025-07-03 02:32:30'),
('Fuel14', 14, '2025-07-03 02:32:30'),
('Fuel15', 15, '2025-07-03 02:32:30');

-- Insert into Vehicle
INSERT INTO Vehicle (UserID, Brand, Model, Year, PlateNumber, FuelTypeID, VehicleTypeID, ImageURL, Status, Mileage, ConditionNote, PricePerHour, PricePerDay, Modified)
VALUES
(1, 'Brand1', 'Model1', 2020, 'MH01AB0001', 1, 1, 'url1.jpg', 'Available', 10100, 'Good', 210.00, 1050.00, '2025-07-03 02:32:30'),
(2, 'Brand2', 'Model2', 2020, 'MH01AB0002', 2, 2, 'url2.jpg', 'Available', 10200, 'Good', 220.00, 1100.00, '2025-07-03 02:32:30'),
(3, 'Brand3', 'Model3', 2020, 'MH01AB0003', 3, 3, 'url3.jpg', 'Available', 10300, 'Good', 230.00, 1150.00, '2025-07-03 02:32:30'),
(4, 'Brand4', 'Model4', 2020, 'MH01AB0004', 4, 4, 'url4.jpg', 'Available', 10400, 'Good', 240.00, 1200.00, '2025-07-03 02:32:30'),
(5, 'Brand5', 'Model5', 2020, 'MH01AB0005', 5, 5, 'url5.jpg', 'Available', 10500, 'Good', 250.00, 1250.00, '2025-07-03 02:32:30'),
(6, 'Brand6', 'Model6', 2020, 'MH01AB0006', 6, 6, 'url6.jpg', 'Available', 10600, 'Good', 260.00, 1300.00, '2025-07-03 02:32:30'),
(7, 'Brand7', 'Model7', 2020, 'MH01AB0007', 7, 7, 'url7.jpg', 'Available', 10700, 'Good', 270.00, 1350.00, '2025-07-03 02:32:30'),
(8, 'Brand8', 'Model8', 2020, 'MH01AB0008', 8, 8, 'url8.jpg', 'Available', 10800, 'Good', 280.00, 1400.00, '2025-07-03 02:32:30'),
(9, 'Brand9', 'Model9', 2020, 'MH01AB0009', 9, 9, 'url9.jpg', 'Available', 10900, 'Good', 290.00, 1450.00, '2025-07-03 02:32:30'),
(10, 'Brand10', 'Model10', 2020, 'MH01AB0010', 10, 10, 'url10.jpg', 'Available', 11000, 'Good', 300.00, 1500.00, '2025-07-03 02:32:30'),
(11, 'Brand11', 'Model11', 2020, 'MH01AB0011', 11, 11, 'url11.jpg', 'Available', 11100, 'Good', 310.00, 1550.00, '2025-07-03 02:32:30'),
(12, 'Brand12', 'Model12', 2020, 'MH01AB0012', 12, 12, 'url12.jpg', 'Available', 11200, 'Good', 320.00, 1600.00, '2025-07-03 02:32:30'),
(13, 'Brand13', 'Model13', 2020, 'MH01AB0013', 13, 13, 'url13.jpg', 'Available', 11300, 'Good', 330.00, 1650.00, '2025-07-03 02:32:30'),
(14, 'Brand14', 'Model14', 2020, 'MH01AB0014', 14, 14, 'url14.jpg', 'Available', 11400, 'Good', 340.00, 1700.00, '2025-07-03 02:32:30'),
(15, 'Brand15', 'Model15', 2020, 'MH01AB0015', 15, 15, 'url15.jpg', 'Available', 11500, 'Good', 350.00, 1750.00, '2025-07-03 02:32:30');

-- Insert into Booking
INSERT INTO Booking (UserID, CustomerID, VehicleID, StartDateTime, EndDateTime, Status, Modified)
VALUES
(1, 1, 1, '2025-07-01 09:00:00', '2025-07-01 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(2, 2, 2, '2025-07-02 09:00:00', '2025-07-02 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(3, 3, 3, '2025-07-03 09:00:00', '2025-07-03 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(4, 4, 4, '2025-07-04 09:00:00', '2025-07-04 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(5, 5, 5, '2025-07-05 09:00:00', '2025-07-05 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(6, 6, 6, '2025-07-06 09:00:00', '2025-07-06 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(7, 7, 7, '2025-07-07 09:00:00', '2025-07-07 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(8, 8, 8, '2025-07-08 09:00:00', '2025-07-08 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(9, 9, 9, '2025-07-09 09:00:00', '2025-07-09 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(10, 10, 10, '2025-07-10 09:00:00', '2025-07-10 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(11, 11, 11, '2025-07-11 09:00:00', '2025-07-11 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(12, 12, 12, '2025-07-12 09:00:00', '2025-07-12 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(13, 13, 13, '2025-07-13 09:00:00', '2025-07-13 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(14, 14, 14, '2025-07-14 09:00:00', '2025-07-14 18:00:00', 'Confirmed', '2025-07-03 02:32:30'),
(15, 15, 15, '2025-07-15 09:00:00', '2025-07-15 18:00:00', 'Confirmed', '2025-07-03 02:32:30');

-- Insert into Agreement
INSERT INTO Agreement (BookingID, UserID, AgreementDate, AgreementPDFPath, Modified)
VALUES
(1, 1, '2025-07-03 02:32:30', 'agreements/agreement1.pdf', '2025-07-03 02:32:30'),
(2, 2, '2025-07-03 02:32:30', 'agreements/agreement2.pdf', '2025-07-03 02:32:30'),
(3, 3, '2025-07-03 02:32:30', 'agreements/agreement3.pdf', '2025-07-03 02:32:30'),
(4, 4, '2025-07-03 02:32:30', 'agreements/agreement4.pdf', '2025-07-03 02:32:30'),
(5, 5, '2025-07-03 02:32:30', 'agreements/agreement5.pdf', '2025-07-03 02:32:30'),
(6, 6, '2025-07-03 02:32:30', 'agreements/agreement6.pdf', '2025-07-03 02:32:30'),
(7, 7, '2025-07-03 02:32:30', 'agreements/agreement7.pdf', '2025-07-03 02:32:30'),
(8, 8, '2025-07-03 02:32:30', 'agreements/agreement8.pdf', '2025-07-03 02:32:30'),
(9, 9, '2025-07-03 02:32:30', 'agreements/agreement9.pdf', '2025-07-03 02:32:30'),
(10, 10, '2025-07-03 02:32:30', 'agreements/agreement10.pdf', '2025-07-03 02:32:30'),
(11, 11, '2025-07-03 02:32:30', 'agreements/agreement11.pdf', '2025-07-03 02:32:30'),
(12, 12, '2025-07-03 02:32:30', 'agreements/agreement12.pdf', '2025-07-03 02:32:30'),
(13, 13, '2025-07-03 02:32:30', 'agreements/agreement13.pdf', '2025-07-03 02:32:30'),
(14, 14, '2025-07-03 02:32:30', 'agreements/agreement14.pdf', '2025-07-03 02:32:30'),
(15, 15, '2025-07-03 02:32:30', 'agreements/agreement15.pdf', '2025-07-03 02:32:30');

-- Insert into Payment
INSERT INTO Payment (BookingID, UserID, Amount, PaymentDate, PaymentMethod, Remarks, Modified)
VALUES
(1, 1, 1010.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(2, 2, 1020.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(3, 3, 1030.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(4, 4, 1040.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(5, 5, 1050.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(6, 6, 1060.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(7, 7, 1070.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(8, 8, 1080.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(9, 9, 1090.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(10, 10, 1100.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(11, 11, 1110.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(12, 12, 1120.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(13, 13, 1130.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(14, 14, 1140.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30'),
(15, 15, 1150.00, '2025-07-03 02:32:30', 'Card', 'Paid in full', '2025-07-03 02:32:30');

-- Insert into MaintenanceLog
INSERT INTO MaintenanceLog (VehicleID, UserID, Description, ServiceDate, Cost, Modified)
VALUES
(1, 1, 'Service task 1', '2025-06-01', 520.00, '2025-07-03 02:32:30'),
(2, 2, 'Service task 2', '2025-06-02', 540.00, '2025-07-03 02:32:30'),
(3, 3, 'Service task 3', '2025-06-03', 560.00, '2025-07-03 02:32:30'),
(4, 4, 'Service task 4', '2025-06-04', 580.00, '2025-07-03 02:32:30'),
(5, 5, 'Service task 5', '2025-06-05', 600.00, '2025-07-03 02:32:30'),
(6, 6, 'Service task 6', '2025-06-06', 620.00, '2025-07-03 02:32:30'),
(7, 7, 'Service task 7', '2025-06-07', 640.00, '2025-07-03 02:32:30'),
(8, 8, 'Service task 8', '2025-06-08', 660.00, '2025-07-03 02:32:30'),
(9, 9, 'Service task 9', '2025-06-09', 680.00, '2025-07-03 02:32:30'),
(10, 10, 'Service task 10', '2025-06-10', 700.00, '2025-07-03 02:32:30'),
(11, 11, 'Service task 11', '2025-06-11', 720.00, '2025-07-03 02:32:30'),
(12, 12, 'Service task 12', '2025-06-12', 740.00, '2025-07-03 02:32:30'),
(13, 13, 'Service task 13', '2025-06-13', 760.00, '2025-07-03 02:32:30'),
(14, 14, 'Service task 14', '2025-06-14', 780.00, '2025-07-03 02:32:30'),
(15, 15, 'Service task 15', '2025-06-15', 800.00, '2025-07-03 02:32:30');

-- 1. Drop Agreement (depends on Booking)
Drop TABLE Agreement;

-- 2. Drop Payment (depends on Booking)
Drop TABLE Payment;

-- 3. Drop MaintenanceLog (depends on Vehicle)
Drop TABLE MaintenanceLog;

-- 4. Drop Booking (depends on Vehicle, Customer, User)
Drop TABLE Booking;

-- 5. Drop Vehicle (depends on VehicleType, FuelType, User)
Drop TABLE Vehicle;

-- 6. Drop VehicleType (depends on User)
Drop TABLE VehicleType;

-- 7. Drop FuelType (depends on User)
Drop TABLE FuelType;

-- 8. Drop Customer (no dependencies)
Drop TABLE Customer;

-- 9. Drop [User] (parent of most tables)
Drop TABLE [User];
 
