import './Login.css';
import React from "react";
import { useState } from "react";
import axios from "axios";
import { Button, Form, Container, Alert, Card, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);
 
        try {
            const response = await axios.post('https://localhost:7219/api/Auth/Login', {
               username: username,
               password: password
                
            });

            localStorage.setItem('token', response.data.token);
            localStorage.setItem('username', response.data.username);
            localStorage.setItem('role', response.data.role);

navigate('/dashboard');

        } catch (error) {
  if (error.response) {
    // Backend responded (401, 500, etc)
    if (error.response.status === 401) {
      setError("Invalid username or password");
    } else {
      setError("Server error. Please try again later.");
    }
  } else if (error.request) {
    // Request sent but no response (backend down)
    setError("Cannot connect to server. Please ensure the backend is running.");
  } else {
    // Something else went wrong
    setError("Unexpected error occurred.");
  }

  console.error(error);
  console.log("Status:", error.response?.status);
console.log("Data:", error.response?.data);

}finally {
          setLoading(false);
        }
    };

    return (
        <Container className="login-container">
          <Card className='login-card'>
            <Card.Body>
              <Card.Title className="login-title">
                 <h1 className="text-muted">Login</h1>
              </Card.Title>
              
              {error && <Alert variant="danger">{error}</Alert>}

            <Form.Floating onSubmit={handleSubmit} >

              <Form.Group className="login-form-group">
                <Form.Label className='login-label'>Username</Form.Label>
                 <Form.Control type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} className='login-input' required/>
              </Form.Group>

              <Form.Group className="login-form-group">
                  <Form.Label className='login-label'>Password</Form.Label>
                  <Form.Control type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} className='login-input' required/>
              </Form.Group>

              <p className='Forgot'>Forgot password?</p>

              <Button variant="primary" type="submit" className="login-button" disabled={loading}>
                    {loading ? <Spinner size="sm" /> : 'Login'}
              </Button>

            </Form.Floating>
          </Card.Body>
        </Card>

      </Container>
    );
};

export default Login;