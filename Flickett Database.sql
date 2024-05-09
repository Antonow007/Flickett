CREATE TABLE Users (
id int AUTO_INCREMENT PRIMARY KEY,
username varchar(50) NOT NULL ,
passwod varchar(50) NOT NULL,
first_name varchar(20) not null,
last_name varchar(40) not null,
email varchar(100) not null,
phone varchar(20) not null,
Role varchar(50)
);


CREATE TABLE Movies (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ApiMovieId VARCHAR(255) UNIQUE,
    Title VARCHAR(255) UNIQUE,
    Overview TEXT,
    PosterUrl VARCHAR(255) UNIQUE,
	Genre VARCHAR(255),
	Duration INT
);

CREATE TABLE Screenings (
    ScreeningId INT AUTO_INCREMENT PRIMARY KEY,
    MovieId VARCHAR(255),
    ScreeningDate DATE,
    ScreeningTime TIME,
    HallId Int
);


 CREATE TABLE Halls (
    HallId INT AUTO_INCREMENT PRIMARY KEY,
    HallName VARCHAR(255) UNIQUE,
    Capacity INT,
    Category VARCHAR(50),
    TicketTypeId INT,
    FOREIGN KEY (TicketTypeId) REFERENCES TicketTypes(TicketTypeId)
);


CREATE TABLE Tickets (
    TicketId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    ScreeningId INT,
    MovieId INT, 
    SeatNumber VARCHAR(20),
    RowNumber varchar(20),
    ReservationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (ScreeningId, SeatNumber) 
);




CREATE TABLE TicketTypes (
    TicketTypeId INT AUTO_INCREMENT PRIMARY KEY,
    TypeName VARCHAR(50) UNIQUE,
    BasePrice DECIMAL(10, 2) NOT NULL
);

