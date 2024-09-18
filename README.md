
# Online Learning Platform

![](wwwroot/img/logo.png)

LearnNest is a web application designed to facilitate online education. The platform allows course providers to create and manage courses, while learners can browse, enroll in, and track their progress through a variety of courses. You can become a Course Provider too and add your own courses for users to see and take.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Setup and Installation](#setup-and-installation)
- [Usage](#usage)
- [License](#license)

## Features

- User registration and authentication
- Course provider functionality to create and manage courses
- Course browsing and enrollment for learners
- Learning progress tracking
- Responsive design for seamless use on different devices

## Technologies Used

- **ASP.NET Core (.NET 6)** – Backend framework for web application development
- **Microsoft SQL Server** – Database for storing user, course, and enrollment data
- **HTML, CSS, JavaScript** – Frontend technologies for user interface design
- **Entity Framework Core** – ORM for data access
- **Bootstrap** – For responsive design

## Setup and Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Eman288/Online-Learning-Platform.git
   cd Online-Learning-Platform
   ```

2. Open the solution in Visual Studio.

3. Configure the database connection string in `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server;Database=OnlineLearning;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. Run the following commands to apply migrations and update the database:

   ```bash
   dotnet ef database update
   ```

5. Build and run the application:

   ```bash
   dotnet run
   ```

## Usage

- **Course Providers** can log in to create, edit, and manage their courses.
- **Learners** can browse available courses, enroll, and track their learning progress.


## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
