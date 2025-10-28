
---

# 🧩 Guide: Using Real ULIDs in PostgreSQL with `pgulid` (Linux, macOS & Windows)

## 📘 Introduction

[ULID (Universally Unique Lexicographically Sortable Identifier)](https://github.com/ulid/spec) is a 128-bit identifier that:

* is **unique** like a UUID,
* is **time-sortable** (lexicographically),
* and is **human-readable** (26 Base32 characters).

The [`pgulid`](https://github.com/geckoboard/pgulid) PostgreSQL extension adds a **native `ulid` type** and a **`gen_ulid()`** function to generate true ULIDs inside the database.

---

## 🧰 Requirements

* PostgreSQL ≥ **11**
* **Superuser** privileges (or permission to run `CREATE EXTENSION`)
* One of the following environments:

  * ✅ Linux
  * ✅ macOS
  * ✅ Windows (via WSL or MSYS2)

---

## ⚙️ Installation

### 🐧 Linux (Ubuntu / Debian)

```bash
sudo apt update
sudo apt install postgresql postgresql-server-dev-all make gcc git
git clone https://github.com/geckoboard/pgulid.git
cd pgulid
make
sudo make install
```

Then enable the extension in PostgreSQL:

```sql
CREATE EXTENSION IF NOT EXISTS pgulid;
```

---

### 🍎 macOS (Homebrew)

```bash
brew install postgresql git make gcc
git clone https://github.com/geckoboard/pgulid.git
cd pgulid
make
sudo make install
```

Then inside `psql`:

```sql
CREATE EXTENSION IF NOT EXISTS pgulid;
```

---

### 🪟 Windows

### 1. Install dependency (pgcrypto) as superuser
Open a brand new query tab in pgAdmin (single statement only), run:
```sql
CREATE EXTENSION IF NOT EXISTS pgcrypto;
```

### 2. Place the Extension Files:

```
C:\Program Files\PostgreSQL\<version>\share\extension\
```

Files to copy from the `config` folder:

* `pgulid--1.0.sql`
* `pgulid.control`

### 3. Create the extension
Open a new query tab (no BEGIN/COMMIT, no other statements) and run as superuser:
```sql
CREATE EXTENSION IF NOT EXISTS pgulid;
```
### 4. Test
```sql
SELECT generate_ulid();
```

---

## 🧪 Usage Examples

### 1. Create a table with a ULID primary key

```sql
CREATE TABLE users (
    id ulid PRIMARY KEY DEFAULT gen_ulid(),
    name TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT now()
);
```

Each new row automatically gets a **unique, time-sortable ULID**.

---

### 2. Insert and query data

```sql
INSERT INTO users (name) VALUES ('John Doe'), ('Jane Smith');
SELECT * FROM users ORDER BY id;
```

Records will appear in chronological order when sorted by `id`.

---

### 3. Generate a ULID manually

```sql
SELECT gen_ulid();
```

Example output:

```
01J9S0S2H8TVA4F2QHCDR9VN1M
```

---

## 🧰 Optional Utilities

### 🔄 Conversions

* Convert Base32 → binary:
  `ulid_in('01J9S0S2H8TVA4F2QHCDR9VN1M')`

* Convert binary → Base32:
  `ulid_out('\x01abcdef...')`

---

### 🔍 Indexing

ULID columns can be indexed normally:

```sql
CREATE INDEX idx_users_ulid ON users (id);
```

---

## 🚀 Benefits of `pgulid`

| Feature                     | Description                                  |
| --------------------------- | -------------------------------------------- |
| 🧱 **Native `ulid` type**   | Acts like a built-in PostgreSQL type         |
| ⚡ **`gen_ulid()` function** | Generates ULIDs following the official spec  |
| 🕒 **Time-sortable**        | Sorts chronologically by ID                  |
| 🧩 **ORM-friendly**         | Works with Prisma, TypeORM, SQLAlchemy, etc. |

---

## 🧹 Uninstalling

```sql
DROP EXTENSION pgulid;
```

---

## 📚 References

* 🔗 [pgulid GitHub Repository](https://github.com/geckoboard/pgulid)
* 📘 [ULID Specification](https://github.com/ulid/spec)
* 🐘 [PostgreSQL Docs – CREATE EXTENSION](https://www.postgresql.org/docs/current/sql-createextension.html)
* 🪟 [WSL Setup Guide (Microsoft)](https://learn.microsoft.com/en-us/windows/wsl/install)
* ⚙️ [MSYS2 Official Site](https://www.msys2.org/)

---

Would you like me to add a **ready PowerShell + Bash setup script** that automates the entire Windows (WSL) installation — including PostgreSQL, build tools, and `pgulid`?
