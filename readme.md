# Job Application Tracker

A web application built with **C# .NET Core** and **React** to help developers track job applications. This project was created to enhance my coding skills while solving a real-world problem.

## Features
- ✅ Track job applications with company details
- ✅ Filter and search applications
- ✅ Modern UI built with React and Material UI

## Planned Features
- 🛠 Application Events (Calls, Emails, Interviews)
- 📊 Charts and Graphs for visual representation of activities
- 📋 Additional Job Details, Descriptions, and Requirements
- 🏗 Containerization with Docker & Docker Compose
- 🔐 Secure user authentication with JWT
- 📅 Add interview dates and job status updates

## Installation

### Prerequisites
- .NET Core 8+
- Node.js 18+
- SQL Server 

### Steps
1. Clone the repository:
   ```sh
   git clone https://github.com/john-smolinski/AppTracker.git
   cd AppTracker
   ```
2. Install backend dependencies:
   ```sh
   cd src
   dotnet restore
   export ASPNETCORE_URLS=http://localhost:5000
   dotnet run
   ```
3. Set up the Database  
   ```sh
   # From `src` directory  
   cd ApplicationTracker.Data  
   dotnet ef database update --startup-project ../ApplicationTracker  
   ```

4. Configure and Run the Backend API  
   ```sh
   # From `src` directory  
   cd ApplicationTracker  
   dotnet run  
   ```
   
5. Install frontend dependencies:
   ```sh
   cd webui/app-tracker
   npm install
   npm start
   ```
6. Open `http://localhost:3000` in your browser.

## Usage
1. Add a new job application with company details.
2. View and filter your applications.


## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact
👤 **John Smolinski**   
🔗 LinkedIn: [john-r-smolinski](https://linkedin.com/in/john-r-smolinski)  
📂 GitHub: [john-smolinski](https://github.com/john-smolinski)
