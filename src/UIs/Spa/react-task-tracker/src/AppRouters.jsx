import { Routes, Route } from "react-router-dom";
import LogIn from "./pages/LogIn.jsx";
import SignUp from "./pages/SignUp.jsx";
import Tasks from "./pages/Tasks/Tasks.jsx";

export function AppRouters() {
    return (
        <Routes>
            <Route path="/login" element={<LogIn />} />
            <Route path="/signup" element={<SignUp />} />
            <Route path="/tasks" element={<Tasks />} />
        </Routes>
    )
}