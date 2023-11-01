const PROXY_CONFIG = [
	{
		context: [
			'/api',
			'/authorize',
			'/.well-known',
			'/connect',
			'/weatherforecast',
		],
		target: 'http://localhost:9100',
		secure: false
	}
];

module.exports = PROXY_CONFIG;
