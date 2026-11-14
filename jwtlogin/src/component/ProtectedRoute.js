import React from "react";
import { Navigate, Outlet } from "react-router-dom";

// This is a user-experience guard only. API endpoints must still enforce
// authorization independently through JWT bearer authentication.
const ProtectedRoute = () => {
    const token = localStorage.getItem("token");

    return token
        ? <Outlet />
        : <Navigate to="/login" replace />;
};

export default ProtectedRoute;
