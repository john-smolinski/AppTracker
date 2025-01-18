import "../src/styles/app.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./components/Home";
import SearchApplications from "./components/SearchApplications";
import AddApplication from "./components/AddApplication";
import NotFound from "./components/NotFound";
import Statistics from "./components/Statistics";

function App() {
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

export default App;
