import {useState} from 'react';
import {OpenTimeHistogram} from './components/OpenTimeHistogram';
import {StatusTimeDistribution} from './components/StatusTimeDistribution';
import {DailyTaskFlow} from './components/DailyTaskFlow';
import {TopUsers} from './components/TopUsers';
import {PriorityDistribution} from './components/PriorityDistribution';
import {WorklogHistogram} from './components/WorklogHistogram';

function App() {
  const [projectKey, setProjectKey] = useState<string>('');
  const [currentProjectKey, setCurrentProjectKey] = useState<string>('');

  const handleLoad = () => {
    if (projectKey.trim()) {
      setCurrentProjectKey(projectKey.trim());
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      handleLoad();
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex gap-4 items-end">
              <div className="flex-1">
                <label htmlFor="projectKey" className="block text-sm font-medium text-gray-700 mb-2">
                  Ключ проекта Apache Jira
                </label>
                <input
                  id="projectKey"
                  type="text"
                  value={projectKey}
                  onChange={(e) => setProjectKey(e.target.value)}
                  onKeyPress={handleKeyPress}
                  placeholder="KAFKA"
                  className="w-full px-4 py-2 border border-gray-300 rounded-md shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
              <button
                onClick={handleLoad}
                disabled={!projectKey.trim()}
                className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
              >
                Загрузить данные
              </button>
            </div>
            {currentProjectKey && (
              <div className="mt-4 text-sm text-gray-600">
                Текущий проект: <span className="font-semibold text-gray-900">{currentProjectKey}</span>
              </div>
            )}
          </div>
        </div>

        {currentProjectKey ? (
          <div className="space-y-8">
            <div className="bg-white rounded-lg shadow-md p-6">
              <OpenTimeHistogram projectKey={currentProjectKey} />
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
              <DailyTaskFlow projectKey={currentProjectKey} />
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
              <PriorityDistribution projectKey={currentProjectKey} />
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
              <StatusTimeDistribution projectKey={currentProjectKey} />
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
              <TopUsers projectKey={currentProjectKey} />
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
              <WorklogHistogram projectKey={currentProjectKey} />
            </div>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow-md p-12 text-center">
            <div className="text-gray-400 mb-4">
              <svg
                className="mx-auto h-24 w-24"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={1.5}
                  d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                />
              </svg>
            </div>
            <h3 className="text-xl font-semibold text-gray-700 mb-2">
              Введите ключ проекта для начала
            </h3>
            <p className="text-gray-500">
              Введите ключ проекта Apache Jira выше и нажмите "Загрузить данные" для просмотра аналитики
            </p>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;

