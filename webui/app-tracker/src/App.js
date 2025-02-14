import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchApplications } from "./redux/applicationsSlice";
import { fetchSources } from "./redux/sourcesSlice";
import { fetchOrganizations } from "./redux/organizataionsSlice";
import { fetchJobTitles } from "./redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "./redux/workEnvironmentsSlice";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./components/Pages/Home/Home";
import Applications from "./components/Pages/Applications/Applications";
import Application from "./components/Pages/Applications/Application/Application";
import AddApplication from "./components/Pages/AddApplication/AddApplication";
import NotFound from "./components/Pages/NotFound/NotFound";
import Statistics from "./components/Pages/Statistics/Statistics";
import "./App.css";

export default function App() {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(fetchApplications());
    dispatch(fetchSources());
    dispatch(fetchOrganizations());
    dispatch(fetchJobTitles());
    dispatch(fetchWorkEnvironments());
  }, [dispatch]);

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/applications" element={<Applications />} />
        <Route path="/applications/:id" element={<Application />} />
        <Route path="/add" element={<AddApplication />} />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}
