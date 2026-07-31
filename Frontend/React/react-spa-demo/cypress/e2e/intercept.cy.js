describe("catalog over a subbed network", () => {
    it("renders exactly the object in our inventory fixture", () => {
        cy.intercept("GET","**/api/Inventory", { fixture:"inventory.json"}).as("getInventory");
        cy.visit("/");
        cy.wait("@getInventory");
        cy.get("article.card").should("have.length", 3);
        cy.contains("article.card", "Stubbed Book Two")
            .find("dd.out")
            .should("have.text", "0");
    });

    it("shows the failure message if the API is down/dead", () => {
        cy.intercept("GET","**/api/Inventory", { statusCode: 500, body:{}}).as("getInventory");
        cy.visit("/");
        cy.wait("@getInventory");
        cy.contains("Could not reach API. Is it running? Check CORS");
    });

});