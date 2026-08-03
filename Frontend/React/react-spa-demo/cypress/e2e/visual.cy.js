describe("catalog visual regression", {browser: "electron"}, () => {
    it("catalog page matches the baseline", () => {
        cy.intercept("GET", "**/api/Inventory", {fixture: "../fixtures/inventory.json"}).as("getInventory");

        cy.visit("/");
        cy.wait("@getInventory");
        cy.get("article.card").should("have.length", 3);

        cy.task("log", "visual: comparing catalog-stubbed against the commited baseline image");

        // cy.document().then((doc) => {
        //     const style = doc.createElement("style");
        //     style.innerHTML = ".card h2 a { color: red; }";
        //     doc.head.appendChild(style);
        // });

        cy.compareSnapshot("catalog-stubbed");
    });
});