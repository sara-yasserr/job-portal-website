# Job Portal System Documentation

## System Overview
A comprehensive job portal built with ASP.NET MVC that connects job seekers with employers through advanced features and secure authentication.

---

## User Roles & Permissions

### 👨‍💼 Job Seeker
- **Profile Management**
  - Create/update personal profile
  - Upload CV and cover letters
- **Job Search**
  - Advanced filters (location, salary, category)
  - Save favorite listings
- **Applications**
  - Track application status
  - Receive notifications

### 👔 Employer
- **Company Profile**
  - Register company account
  - Upload company logo/details
- **Job Postings**
  - Create/edit job listings
  - Manage active/inactive posts
- **Applications**
  - Review candidate submissions
  - Update application status

### 👑 Administrator
- **User Management**
  - Activate/deactivate accounts
  - Reset user passwords
- **Content Moderation**
  - Approve/reject job postings
  - Manage categories
- **System Configuration**
  - Email templates
  - Security settings

---

## Technical Specifications

### 🔧 System Architecture
ASP.NET MVC (Model-View-Controller)
├── Presentation Layer (Views)
├── Business Logic (Controllers)
└── Data Access (Models + Entity Framework)


### 🗃️ Database Tables
| Table        | Key Fields                          |
|--------------|-------------------------------------|
| Users        | Id, Email, PasswordHash, Role       |
| Profiles     | UserId, FullName, Skills, Experience|
| Jobs         | Title, Description, Salary, Expiry  |
| Applications | JobId, UserId, Status, ApplyDate   |

---

## Authentication Flow

### 🔐 Login Options
1. **Email/Password**
   - Secure password hashing
   - Account lockout after 5 attempts

2. **External Providers**
   ```bash
   # Google Authentication Setup:
   dotnet user-secrets set "Authentication:Google:ClientId" "339719528786-br8jgs2deqhbtge4o01ffg14lls7td0c.apps.googleusercontent.com"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "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"
Password Recovery

Token-based reset system

SMS/Email verification

Setup Guide
🛠️ Installation Steps
Clone repository:

bash
git clone https://github.com/your-repo/job-portal.git
Configure database:

bash
dotnet ef database update
Set authentication:

bash
# For development secrets
dotnet user-secrets init
Run application:

bash
dotnet run
Security Measures
✅ All sensitive data encrypted

🔄 Regular dependency updates

📜 Audit logs for all admin actions

🛡️ HTTPS enforcement

🕵️‍♂️ SQL injection protection

