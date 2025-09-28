import { useState, useEffect } from 'react'
import { OpenFeature } from '@openfeature/web-sdk'
import { FlagdWebProvider } from '@openfeature/flagd-web-provider'
import Home from './components/Home'
import './App.css'

function App() {
  const [isFeatureFlagsReady, setIsFeatureFlagsReady] = useState(false)

  useEffect(() => {
    const initializeFeatureFlags = async () => {
      try {
        // Set up the OpenFeature provider (flagd/goff)
        // This should match the goff service endpoint configured in Aspire
        const flagdProvider = new FlagdWebProvider({
          endpoint: 'http://localhost:1031' // Default goff endpoint in Aspire
        })
        
        await OpenFeature.setProvider(flagdProvider)
        console.log('OpenFeature provider initialized')
      } catch (error) {
        console.warn('Failed to initialize OpenFeature provider:', error)
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