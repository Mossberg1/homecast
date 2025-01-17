# HomeCast

A self-hosted API for managing and streaming movies and shows. Designed for learning purposes, this project provides an opportunity to explore API development, authentication, and media streaming functionality. A web interface will be added in future updates.

---

## Features

- Stream movies and shows directly through the API.
- Manage your media library, including movies and TV shows.
- User authentication and access control.
- Upload and organize media files into dedicated folders.
- Easy setup for local development.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQLite](https://sqlite.org/) (pre-installed on most platforms)
- Basic familiarity with .NET Core and command-line tools.

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/streaming-api.git
   cd homecast
   ```

2. **Install dependencies:**

   ```bash
   dotnet restore
   ```

3. **Set up the database:**

   ```bash
   dotnet ef database update
   ```

4. **Run the application:**

   ```bash
   dotnet run
   ```

5. **Access the API:** Open your browser and navigate to the url specified in the terminal.

---

## Development Setup

- In **Development** mode, the application will automatically create the following folders if they don't exist:
  - `MediaStorage/Movies`
  - `MediaStorage/Shows`

---

## API Documentation

The API includes built-in documentation via Swagger. To explore and test API endpoints:

- Navigate to `https://localhost:<portnumber>/swager/index.html` after starting the application.

---

## Future Plans

- Add a user-friendly web interface for browsing and streaming.
- Implement raiting system.
- Implement recommendation system.
- Integrate metadata from external providers like `https://www.themoviedb.org/`.

---

## Learning Focus

This project is intended for me to learn more about:

- Building APIs with ASP.NET Core.
- Building web pages with blazor.
- Implementing authentication and authorization with access control.
- Managing media libraries.
- Implement media streaming.
- Preparing a project for self-hosting and scalability.

---

## License



