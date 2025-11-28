import {useEffect, useState} from 'react';
import {Cell, Legend, Pie, PieChart, ResponsiveContainer, Tooltip} from 'recharts';
import {jiraApi} from '../services/api';
import type {PriorityDistributionDto} from '../types/api';

interface PriorityDistributionProps {
  projectKey: string;
}

const COLORS = ['#ef4444', '#f59e0b', '#3b82f6', '#10b981', '#8b5cf6', '#ec4899'];

export function PriorityDistribution({ projectKey }: PriorityDistributionProps) {
  const [data, setData] = useState<PriorityDistributionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await jiraApi.getPriorityDistribution(projectKey);
        setData(response);
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

  if (!data) return null;

  const chartData = data.distribution.map(item => ({
    name: item.priority,
    value: item.count,
    percentage: item.percentage,
  }));

  return (
    <div className="w-full h-96">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Распределение по приоритетам (Всего: {data.totalIssues})
      </h3>
      <ResponsiveContainer width="100%" height="100%">
        <PieChart margin={{ top: 20, right: 30, left: 20, bottom: 20 }}>
          <Pie
            data={chartData}
            cx="50%"
            cy="50%"
            labelLine={false}
            label={({ name, percentage }) => `${name}: ${percentage.toFixed(1)}%`}
            outerRadius={100}
            fill="#8884d8"
            dataKey="value"
          >
            {chartData.map((_, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
            ))}
          </Pie>
          <Tooltip formatter={(value: number) => `${value} задач`} />
          <Legend wrapperStyle={{ paddingTop: '20px' }} />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}

