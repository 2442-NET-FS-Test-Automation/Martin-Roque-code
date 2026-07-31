describe("login", () => {
    beforeEach(() =>{
        cy.visit("/login");
    });

    it("signs in the seeded admin and updates the header", () => {
        cy.contains("label", "Username").find("input").type("ada");
        cy.contains("label", "Password").find("input").type("pass123!");
        cy.contains("button", "Sign in").click();
        cy.contains(".auth-box span", "ada (admin)");
        cy.contains("button", "Sign out");
        cy.contains("nav a", "Admin");
    });

    it("shows the error message for bad credentials", () => {
        cy.contains("label", "Username").find("input").type("ada");
        cy.contains("label", "Password").find("input").type("wrong-password");
        cy.contains("button", "Sign in").click();
        cy.get("p.error").should("have.text", "Invalid username or password");
        cy.url().should("include", "/login");
        cy.contains(".auth-box a", "Sign in");
    });
});