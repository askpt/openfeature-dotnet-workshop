import { useState, useEffect } from 'react'
import { OpenFeature } from '@openfeature/web-sdk'
import { OFREPWebProvider } from '@openfeature/ofrep-web-provider'
import Home from './components/Home'
import './App.css'

// Extend the Window interface to include our environment variables
declare global {
  interface Window {
    env?: {
      OFREP_ENDPOINT?: string;
    };
  }
}

function App() {
  const [isFeatureFlagsReady, setIsFeatureFlagsReady] = useState(false)

  useEffect(() => {
    const initializeFeatureFlags = async () => {
      try {
        // Get the OFREP endpoint from environment variables injected by the server
        const ofrepEndpoint = window.env?.OFREP_ENDPOINT || 'http://localhost:1031/ofrep/v1/'
        
        console.log('Using OFREP endpoint from environment:', ofrepEndpoint)
        
        // Set up the OpenFeature provider using OFREP with the environment-configured endpoint
        const ofrepProvider = new OFREPWebProvider({
          baseUrl: ofrepEndpoint,
        })
        
        await OpenFeature.setProvider(ofrepProvider)
        console.log('OFREP OpenFeature provider initialized successfully')
      } catch (error) {
        console.warn('Failed to initialize OpenFeature provider:', error)
        // Fallback - try to initialize with default endpoint
        try {
          const ofrepProvider = new OFREPWebProvider({
            baseUrl: 'http://localhost:1031/ofrep/v1/',
          })
          await OpenFeature.setProvider(ofrepProvider)
          console.log('OFREP OpenFeature provider initialized with fallback endpoint')
        } catch (fallbackError) {
          console.error('Failed to initialize OpenFeature provider with fallback:', fallbackError)
        }
      } finally {
        setIsFeatureFlagsReady(true)
      }
    }

    initializeFeatureFlags()
  }, [])

  if (!isFeatureFlagsReady) {
    return <div className="loading">Initializing...</div>
  }

  return (
    <div className="App">
      <Home />
    </div>
  )
}

export default App