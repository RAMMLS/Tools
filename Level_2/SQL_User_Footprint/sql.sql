    CREATE DATABASE IF NOT EXISTS your_database;
    USE your_database;

    CREATE TABLE IF NOT EXISTS user_activity (
        id INT AUTO_INCREMENT PRIMARY KEY,
        timestamp DATETIME NOT NULL,
        user VARCHAR(255) NOT NULL,
        action TEXT,
        details TEXT
    );

