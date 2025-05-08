Function Point Calculator
A web-based tool to automate Function Point Analysis (FPA) with AI-powered SRS analysis and SQL Server integration.

📋 Overview
This project calculates Function Points (FP) for software size estimation, integrating the Gemini 1.5-flash API to analyze Software Requirements Specifications (SRS) and store results in a SQL Server database.

🚀 Features
- Automated FPA: Computes Unadjusted Function Points (UFP), Value Adjustment Factor (VAF), and Adjusted Function Points (FP).
- AI SRS Analysis: Analyzes SRS files (PDF, TXT, JPG) using Gemini 1.5-flash API.
- Data Storage: Saves results in a SQL Server database.
- PDF Export: Generates downloadable FP reports.
- User-Friendly: Responsive design with Bootstrap and intuitive input forms.

🛠️ Technologies
-Frontend: HTML, CSS, JavaScript, Bootstrap 4.5.0
- Backend: C# (ASP.NET 4.6.1), iTextSharp (PDF generation)
- Database: SQL Server
- AI: Gemini 1.5-flash API
- Tools: Visual Studio 2022

📂 Project Structure

FunctionPoint/UpdateFunctionPointCalculator/FunctionPointCalculator/
- Defaults.aspx      # Main UI page
- Defaults.aspx.cs   # Backend logic
- Web.config         # Configuration file
- README.md                  # This file

⚙️ Setup Instructions
1. Prerequisites
- SQL Server
- Visual Studio 2022
- .NET Framework 4.6.1
- Gemini API Key

2. Database Setup
CREATE DATABASE FunctionPointDB;
GO

USE FunctionPointDB;
GO

CREATE TABLE FunctionPointResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UFP DECIMAL(18,2),
    VAF DECIMAL(18,2),
    FP DECIMAL(18,2),
    CalculationDate DATETIME,
    SrsFileName NVARCHAR(255) NULL,
    SrsAnalysisResult NVARCHAR(MAX) NULL
);

SELECT * FROM FunctionPointResults;

3. Configure Web.config
Update Web.config with your settings:
<connectionStrings>
    <add name="FunctionPointDB" connectionString="Server=[YourServer];Database=FunctionPointDB;User Id=[YourUser];Password=[YourPassword];" providerName="System.Data.SqlClient" />
</connectionStrings>
<appSettings>
    <add key="AiApiKey" value="[YourGeminiApiKey]" />
</appSettings>


Note: Replace placeholders with your actual server details and API key. Avoid committing sensitive data.

4. Run the Project
- Open in Visual Studio 2022.
- Build and run (F5).


📋 Usage
-  Upload SRS: Upload a PDF, TXT, or JPG SRS file and click "Analyze with AI".
- Input Data: Enter FP components (EI, EO, EQ, ILF, EIF) and VAF factors (F1-F14).
- Calculate: Click "Calculate FP" to get results.
- Export: Click "Export PDF Report" to download results.
- Reset: Click "Reset" to clear inputs.


🌟 Advantages
+ Automates complex FPA calculations.
+ AI enhances SRS analysis efficiency.
+ Stores and exports results easily.

🚧 Limitations
- Accuracy depends on SRS clarity.
- AI may misinterpret ambiguous inputs.

🔮 Future Improvements
- Support more file formats.
- Integrate better AI models to analyze SRS documents and produce more accurate results
- Add multi-language support.

📚 References
- CMU-CS 462 - Bang in - Full 15 Lectures.pdf
- IFPUG (2009). IFPUG Standards: Function Point Analysis. Available at: https://ifpug.org/ifpug-standards/fpa
- Gemini API Documentation: developers.generativeai.google


