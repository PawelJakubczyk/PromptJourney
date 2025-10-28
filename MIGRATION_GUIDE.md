
# 🧩 Entity Framework Core – Migration Commands

## Apply migrations
```bash
Update-Database -Project Persistence -StartupProject App
````

## Remove migrations

```bash
cd .\src\Persistence
rm -r Migrations
```
Or use
```bash
rm -r .\src\Persistence\Migrations
```

## Generate new migrations

```bash
Add-Migration migration_init -Project Persistence -StartupProject App
Add-Migration migration_seeding -Project Persistence -StartupProject App
Add-Migration migration_update -Project Persistence -StartupProject App
```

## Revert migrations

```bash
Remove-Migration -Project Persistence -StartupProject App
Remove-Migration -Project Persistence -StartupProject App -Force
Remove-Migration -Project Persistence -StartupProject App -Name migration_init
```

## Generate SQL migration script

```bash
Script-Migration -Project Persistence -StartupProject App
Script-Migration -Project Persistence -StartupProject App -From 20231020123456_migration_init -To 20231020123457_migration_init2
Script-Migration -Project Persistence -StartupProject App -Idempotent
```

## Apply migrations to the database

```bash
Update-Database -Project Persistence -StartupProject App migration_init
```

## Apply or revert to a specific migration

```bash
Update-Database -Project Persistence -StartupProject App -Migration 20231020123456_migration_init
```

---

**Tips:**

* Use `-Idempotent` when generating scripts for environments where the DB state may vary.
* Always verify migration order before applying in production.
* Keep migration names descriptive (e.g., `migration_add_user_table`).


