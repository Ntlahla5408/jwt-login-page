import React from "react";
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link } from "react-router-dom";

const Dashboard = () => {
    return(
    <Navbar bg="dark" variant="dark">
      <Container>
        <Navbar.Brand>Dashboard</Navbar.Brand>
        <Nav className="me-auto">
          <Nav.Link as={Link} to="/dashboard">Home</Nav.Link>
        </Nav>
      </Container>
    </Navbar>
    );
};

export default Dashboard;