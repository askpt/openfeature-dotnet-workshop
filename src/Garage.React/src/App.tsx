import { useState, useEffect } from "react";
import Home from "./components/Home";
import "./App.css";

function App() {
  const [isFeatureFlagsReady, setIsFeatureFlagsReady] = useState(false);

  useEffect(() => {
    const initializeFeatureFlags = async () => {
      try {
        console.log("OFREP OpenFeature provider initialized with goff service");
      } catch (error) {
        console.warn("Failed to initialize OpenFeature provider:", error);
      } finally {
        setIsFeatureFlagsReady(true);
      }
    };

    initializeFeatureFlags();
  }, []);

  if (!isFeatureFlagsReady) {
    return <div className="loading">Initializing...</div>;
  }

  return (
    <div className="App">
      <Home />
    </div>
  );
}

export default App;
