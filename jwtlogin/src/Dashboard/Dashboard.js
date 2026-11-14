import React from "react";
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import Button from 'react-bootstrap/Button';
import { useNavigate } from "react-router-dom";

const Dashboard = () => {
    const navigate = useNavigate();
    const username = localStorage.getItem("username");
    const role = localStorage.getItem("role");

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("username");
        localStorage.removeItem("role");
        navigate("/login", { replace: true });
    };

    return (
        <Navbar bg="dark" variant="dark">
            <Container>
                <Navbar.Brand>Dashboard</Navbar.Brand>
                <Nav className="me-auto">
                    <Navbar.Text className="me-3">
                        {username ? `Signed in as ${username}` : "Signed in"}
                        {role ? ` (${role})` : ""}
                    </Navbar.Text>
                </Nav>
                <Button variant="outline-light" onClick={handleLogout}>
                    Logout
                </Button>
            </Container>
        </Navbar>
    );
};

export default Dashboard;
