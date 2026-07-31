describe("catalog filtering and sorting", () => {
    beforeEach(() => {
        cy.visit("/");
        cy.get("article.card").should("have.length.at.least", 3); 
    });

    it("filters by name as the user types", () => {
        cy.get('input[type="search"][placeholder="Filter by name..."]').type("clean");
        cy.get("article.card").should("have.length", 1);
        cy.get("article.card h3 a").should("contain.text", "Clean Code");
    }); 

    it("shows empty state for bad search", () => {
        cy.get('input[type="search"]').type("zzz");
        cy.contains('No books match zzz');
    })

    it("sorts Z-A and back", () => {
        cy.get("article.card h3 a").first().should("contain.text", "Clean Code");
        cy.contains("button", "Sort Z-A").click();
        cy.get("article.card h3 a").first().should("contain.text", "The Pragmatic Programmer");
        cy.contains("button", "Sort A-Z").click();
        cy.get("article.card h3 a").first().should("contain.text", "Clean Code");
    });

    it("links every card to its detail route", () => {
        cy.get("article.card").first().find("h3 a")
            .should("have.attr", "href")
            .and("include", "/inventory/");
    });
});