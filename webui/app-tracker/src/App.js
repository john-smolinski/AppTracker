import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchApplications } from "./redux/applicationSlice";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./components/Pages/Home/Home";
import SearchApplications from "./components/Pages/Search/SearchApplications";
import AddApplication from "./components/Pages/AddApplication/AddApplication";
import NotFound from "./components/Pages/NotFound/NotFound";
import Statistics from "./components/Pages/Statistics/Statistics";
import "./App.css";

export default function App() {
  const dispatch = useDispatch();

  useEffect(() => {
    // fetch application data on start
    dispatch(fetchApplications());
  }, [dispatch]);

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/search" element={<SearchApplications />} />
        <Route path="/add" element={<AddApplication />} />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}
