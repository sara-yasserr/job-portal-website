# Job Portal System Documentation

## Authentication Flow

### 🔐 Login Options
1. *Email/Password*
   - Secure password hashing

2. *External Providers*
   ```bash
   # Google Authentication Setup:
   dotnet user-secrets set "Authentication:Google:ClientId" "339719528786-br8jgs2deqhbtge4o01ffg14lls7td0c.apps.googleusercontent.com"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "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"
Password Recovery

Token-based reset system


Setup Guide
🛠 Installation Steps
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

🛡 HTTPS enforcement

🕵‍♂ SQL injection protection


## System Overview
A comprehensive job portal built with ASP.NET MVC that connects job seekers with employers through advanced features and secure authentication.

---

## User Roles & Permissions

### 👨‍💼 Job Seeker
- *Profile Management*
  - Create/update personal profile
  - Upload CV and profile photo
  - Reset Password
- *Job Search*
  - Advanced filters (Work mode, experience, category)
  - Save bookmarked jobs
  - Easy Apply option with default CV and apply option with specific CV
- *Applications*
  - Track application status
  - Edit / Delete Application

### 👔 Employer
- *Company Profile*
  - Register company account
  - Upload company logo/details
- *Job Postings*
  - Create/edit job listings
  - Manage active/inactive posts
- *Applications*
  - Review candidate submissions
  - Update application status

### 👑 Administrator
- *User Management*
  - Activate/deactivate accounts
  - Reset user passwords
- *Content Moderation*
  - Manage job postings
  - Manage companies
  - Manage categories
- *System Configuration*
  - Email templates
  - Security settings

---

### 🔧 System Architecture
ASP.NET MVC (Model-View-Controller)
├── Presentation Layer (Views)
├── Business Logic (Controllers)
└── Data Access (Models + Entity Framework)
