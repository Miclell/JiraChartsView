import {useEffect, useState} from 'react';
import {Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts';
import {jiraApi} from '../services/api';

interface StatusTimeDistributionProps {
  projectKey: string;
}

interface HistogramData {
  range: string;
  count: number;
}

interface StatusHistogram {
  status: string;
  data: HistogramData[];
}

export function StatusTimeDistribution({ projectKey }: StatusTimeDistributionProps) {
  const [statusHistograms, setStatusHistograms] = useState<StatusHistogram[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await jiraApi.getStatusTimeDistribution(projectKey);
        
        if (!response || !response.issues || response.issues.length === 0) {
          setStatusHistograms([]);
          setLoading(false);
          return;
        }
        
        // Calculate time spent in each status (only for closed tasks)
        // We need to filter only closed tasks - they should have statusChanges ending with "Closed" status
        const statusTimes: Record<string, number[]> = {};
        
        response.issues.forEach(issue => {
          if (!issue.created) return;
          
          // Only process closed tasks - check currentStatus
          const currentStatusLower = (issue.currentStatus || '').toLowerCase();
          const isClosed = currentStatusLower.includes('closed') ||
                          currentStatusLower.includes('done') ||
                          currentStatusLower.includes('resolved') ||
                          currentStatusLower.includes('закрыт') ||
                          currentStatusLower.includes('решено');
          
          // Skip if not closed
          if (!isClosed) return;
          
          const created = new Date(issue.created).getTime();
          const resolutionDate = issue.resolutionDate ? new Date(issue.resolutionDate).getTime() : null;
          
          if (issue.statusChanges.length === 0) {
            // No status changes but task is closed - use time from creation to resolution
            // We'll use a generic "Initial Status" or "Open" status
            if (resolutionDate) {
              const timeInStatus = (resolutionDate - created) / (1000 * 60 * 60 * 24); // days
              if (timeInStatus > 0) {
                const statusName = issue.currentStatus || 'Open';
                if (!statusTimes[statusName]) {
                  statusTimes[statusName] = [];
                }
                statusTimes[statusName].push(timeInStatus);
              }
            }
            return;
          }
          
          // Time in initial status (from creation to first change)
          const firstChange = issue.statusChanges[0];
          const firstChangeTime = new Date(firstChange.changeDate).getTime();
          const initialStatus = firstChange.fromStatus;
          const timeInInitialStatus = (firstChangeTime - created) / (1000 * 60 * 60 * 24); // days
          
          if (timeInInitialStatus > 0 && initialStatus) {
            if (!statusTimes[initialStatus]) {
              statusTimes[initialStatus] = [];
            }
            statusTimes[initialStatus].push(timeInInitialStatus);
          }
          
          // Time in each status between changes
          for (let i = 0; i < issue.statusChanges.length - 1; i++) {
            const currentChange = issue.statusChanges[i];
            const nextChange = issue.statusChanges[i + 1];
            const currentChangeTime = new Date(currentChange.changeDate).getTime();
            const nextChangeTime = new Date(nextChange.changeDate).getTime();
            const status = currentChange.toStatus;
            const timeSpent = (nextChangeTime - currentChangeTime) / (1000 * 60 * 60 * 24); // days
            
            if (timeSpent > 0 && status) {
              if (!statusTimes[status]) {
                statusTimes[status] = [];
              }
              statusTimes[status].push(timeSpent);
            }
          }
          
          // Time in last status (from last change to resolution)
          const lastChange = issue.statusChanges[issue.statusChanges.length - 1];
          const lastChangeTime = new Date(lastChange.changeDate).getTime();
          const lastStatus = lastChange.toStatus;
          
          // Use resolution date if available, otherwise skip
          if (issue.resolutionDate && lastStatus) {
            const resolutionTime = new Date(issue.resolutionDate).getTime();
            const timeInLastStatus = (resolutionTime - lastChangeTime) / (1000 * 60 * 60 * 24); // days
            
            if (timeInLastStatus > 0) {
              if (!statusTimes[lastStatus]) {
                statusTimes[lastStatus] = [];
              }
              statusTimes[lastStatus].push(timeInLastStatus);
            }
          }
        });

        // Create histograms for each status
        const bins = [0, 1, 3, 7, 14, 30, 60, 90, Infinity];
        const histograms: StatusHistogram[] = Object.entries(statusTimes)
          .filter(([_, times]) => times.length > 0)
          .map(([status, times]) => {
            const histogram: HistogramData[] = bins.slice(0, -1).map((start, index) => {
              const end = bins[index + 1];
              const count = times.filter(time => time >= start && time < end).length;
              const range = end === Infinity 
                ? `${start}+ дней` 
                : start === 0 && end === 1
                ? `<1 дня`
                : `${start}-${end} дней`;
              return { range, count };
            });
            
            return {
              status,
              data: histogram.filter(item => item.count > 0) // Only show ranges with data
            };
          })
          .filter(h => h.data.length > 0);

        setStatusHistograms(histograms);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Ошибка загрузки данных');
      } finally {
        setLoading(false);
      }
    };

    if (projectKey) {
      fetchData();
    }
  }, [projectKey]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Загрузка...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Ошибка: {error}</div>
      </div>
    );
  }

  if (statusHistograms.length === 0) {
    return (
      <div className="w-full">
        <h3 className="text-xl font-semibold mb-4 text-gray-800">Распределение времени по статусам (только закрытые задачи)</h3>
        <div className="flex items-center justify-center h-64">
          <div className="text-gray-500">Нет данных для отображения</div>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full space-y-8">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">Распределение времени по статусам (только закрытые задачи)</h3>
      {statusHistograms.map((histogram) => (
        <div key={histogram.status} className="bg-white rounded-lg shadow-md p-6">
          <h4 className="text-lg font-semibold mb-4 text-gray-700">Статус: {histogram.status}</h4>
          <div className="h-96">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={histogram.data} margin={{ top: 20, right: 30, left: 20, bottom: 100 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis 
                  dataKey="range" 
                  angle={-45} 
                  textAnchor="end" 
                  height={100}
                  interval={0}
                  tick={{ fontSize: 12 }}
                />
                <YAxis label={{ value: 'Количество задач', angle: -90, position: 'insideLeft' }} />
                <Tooltip />
                <Legend />
                <Bar dataKey="count" fill="#3b82f6" name="Количество задач" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      ))}
    </div>
  );
}
