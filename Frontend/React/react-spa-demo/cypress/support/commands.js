Cypress.Commands.add("resetInventory", () => {
    // NOTE: the minimal API's route is literally "/inventory/rest" (typo in Program.cs)
    cy.request("POST", "http://localhost:5172/inventory/rest")
});

Cypress.Commands.add("login", (username, password) => {
    cy.request("POST", "http://localhost:5045/auth/login", { username, password })
        .then(({body}) => {
            window.localStorage.setItem("library.token", body.token)
        });
});