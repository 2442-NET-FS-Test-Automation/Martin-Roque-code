//Page Object Model (POM): One class per page of my SPA (or MPA)
//Selectors and page actions live in this file

export class CatalogPage {
    //First, a method to test "readiness"
    visit() {
        cy.visit("/");
        cy.get("article.card").should("have.length.at.least", 1);
        return this;
    }

    search(text) {
        cy.get('input[type="search"][placeholder="Filter by name..."]').type(text);
        return this;
    }

    toggleSort() {
        cy.get(".toolbar button").click();
        return this;
    }

    cards() {
        return cy.get("article.card");
    }

    firstTitle() {
        return cy.get("article.card h3 a").first();
    }
}