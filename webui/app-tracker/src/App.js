import "../src/styles/app.css";
import ApplicationsList from "./components/ApplicationsList";
import Summary from "./components/Summary";
import Menu from "./components/Menu";
import ApplicationCalendar from "./components/ApplicationCalendar";

function App() {
  return (
    <div className="app">
      <Menu />
      <Summary />
      {/*<ApplicationsList />*/}
      <ApplicationCalendar />
    </div>
  );
}

export default App;
