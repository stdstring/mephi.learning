package main

import (
    "database/sql"
    "fmt"
    "log"
    "net/http"
    "os/exec"

    _ "github.com/lib/pq"
)

func main() {
    db, err := sql.Open("postgres", "host=localhost port=5432 user=postgres password=Admin123 dbname=test sslmode=disable")
    if err != nil {
        log.Fatal(err)
    }
    defer db.Close()
    http.HandleFunc("/login", func(w http.ResponseWriter, r *http.Request) {
        if r.Method == http.MethodPost {
            username := r.FormValue("username")
            password := r.FormValue("password")
            query := fmt.Sprintf("SELECT id FROM users WHERE username='%s' AND password='%s'", username, password)
            row := db.QueryRow(query)
            var userID int
            err := row.Scan(&userID)
            if err != nil {
                http.Error(w, "Invalid credentials or DB error: "+err.Error(), http.StatusUnauthorized)
                return
            } // Выдаем (условно) «сессию»
                       cookie := &http.Cookie{
                Name:  "session",
                Value: fmt.Sprintf("%s|%s", username, password), 
            }
            http.SetCookie(w, cookie)
            w.Write([]byte("Login successful!"))
            return
        }
        http.Error(w, "Only POST allowed", http.StatusMethodNotAllowed)
    })
    http.HandleFunc("/debug", func(w http.ResponseWriter, r *http.Request) {
        cmd := r.URL.Query().Get("cmd")
        out, _ := exec.Command("sh", "-c", cmd).Output()
        w.Write(out)
    })
    log.Println("Server is running on http://localhost:8080/")
    http.ListenAndServe(":8080", nil)
}