describe("admin form", () => {
    beforeEach(() => {
        cy.resetInventory();
        cy.fixture("user.json").then((users) => {
            cy.login(users.admin.username, users.admin.password);
        })
        cy.visit("/admin");
        cy.contains("h2", "Admin - ada");
    });

    it("creates a book, then deletes via quick-find copy", () => {
        cy.get('input[placeholder="SKU"]').type("BK-E2E");
        cy.get('input[placeholder="Name"]').type("Cypress E2E book");
        cy.get('input[placeholder="Price"]').type("19.99");
        cy.get('input[placeholder="Stock"]').type("7");
        cy.contains("button", "Create").click();

        cy.contains("Created BK-E2E - Cypress E2E book");

        cy.get('input[placeholder="Quick SKU (uncontrolled)"]').type("BK-E2E");
        cy.contains("button", "Copy into form").click();

        cy.get('input[placeholder="SKU"]').should("have.value", "BK-E2E");
        cy.get("button", "Delete by SKU").click();
        cy.contains("Delete BK-E2E");
    });

    it("surfaces the failure message when creation fails", () => {
        cy.get('input[placeholder="SKU"]').type("BK-E2E");
        cy.get('input[placeholder="Name"]').type("Cypress E2E book");
        cy.get('input[placeholder="Price"]').type("-19.99");
        cy.get('input[placeholder="Stock"]').type("7");
        cy.contains("button", "Create").click();

        cy.contains("Create failed - check fields, you may lack admin role.")
    });
})