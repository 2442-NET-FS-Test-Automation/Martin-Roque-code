import { SearchBar} from "../../src/components/SearchBar";
//cy.spy, same functionality as moq.verify

describe("SearchBar (component)", () => {
    it("renders the value passed in by parent", () => {
        cy.mount(<SearchBar value="clean" onChange={() => {}}/>);

        cy.get('input[type=search]').should("have.value", "clean");
    });

    it("reports every keystroke to the parent", () => {
        const onChange = cy.spy().as("onChange");

        cy.mount(<SearchBar value="" onChange={onChange}/>);

        cy.get("input[type=search]").type("dune");

        cy.get("@onChange").should("have.callCount", 4);
        cy.get("@onChange").should("have.been.calledWith", "d");
        //cy.get("@onChange").should("have.been.calledWith", "u");
        //cy.get("@onChange").should("have.been.calledWith", "n");
        cy.get("@onChange").should("have.been.calledWith", "e");
    });
});