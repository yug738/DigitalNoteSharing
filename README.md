# 📚 Digital Note Sharing Platform

A web-based platform where students can **upload, manage, and explore digital notes** across various file formats.  
Includes built-in **file conversion tools** (CSV ↔ XLSX, PDF ↔ Word, PPTX → PDF, Image → PDF) and **user authentication** with role-based access.

---

## 🚀 Features

### 🧾 Note Management
- Upload, edit, and delete your notes  
- Supported formats: `.pdf`, `.pptx`, `.docx`, `.xlsx`, `.csv`, `.png`, `.jpg`  
- Organized library with filters and search (by title, degree, major, etc.)  

### 🔐 User Authentication
- ASP.NET Core Identity integration  
- Secure registration & login  
- Custom user metadata (Name, College, Degree, Department, Year/Semester)  

### 🔄 File Conversion
Convert files directly within the app:
- CSV ↔ XLSX  
- PDF ↔ Word  
- PPTX → PDF  
- Image → PDF  

### 🧠 Tech Stack
- **Backend:** ASP.NET Core 8 MVC  
- **Frontend:** Razor Pages + Bootstrap 5  
- **Database:** SQL Server (EF Core ORM)  
- **Libraries:**  
  - EPPlus (Excel operations)  
  - Spire.Doc / Spire.PDF / Spire.Presentation (Document conversion)  
  - Identity for authentication  

---

## ⚙️ Setup Instructions

1. Clone the repository  
   ```bash
   git clone https://github.com/<your-username>/DigitalNoteSharing.git
   cd DigitalNoteSharing
# DigitalNoteSharing
